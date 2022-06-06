using System;
using System.IO;
using System.Web;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Xml.Serialization;
using ThoughtWorks.QRCode.Codec;
using System.Web.Configuration;

namespace VMall.Core
{
    /// <summary>
    /// IO帮助类
    /// </summary>
    public class IOHelper
    {
        private static object ctx = new object();//锁对象
        //是否已经加载了JPEG编码解码器
        private static bool _isloadjpegcodec = false;
        //当前系统安装的JPEG编码解码器
        private static ImageCodecInfo _jpegcodec = null;

        /// <summary>
        /// 获得文件物理路径
        /// </summary>
        /// <returns></returns>
        public static string GetMapPath(string path)
        {
            if (HttpContext.Current != null)
            {
                return HttpContext.Current.Server.MapPath(path);
            }
            else
            {
                return System.Web.Hosting.HostingEnvironment.MapPath(path);
            }
        }

        #region  序列化

        /// <summary>
        /// XML序列化
        /// </summary>
        /// <param name="obj">序列对象</param>
        /// <param name="filePath">XML文件路径</param>
        /// <returns>是否成功</returns>
        public static bool SerializeToXml(object obj, string filePath)
        {
            bool result = false;

            FileStream fs = null;
            try
            {
                fs = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
                XmlSerializer serializer = new XmlSerializer(obj.GetType());
                serializer.Serialize(fs, obj);
                result = true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (fs != null)
                    fs.Close();
            }
            return result;

        }

        /// <summary>
        /// XML反序列化
        /// </summary>
        /// <param name="type">目标类型(Type类型)</param>
        /// <param name="filePath">XML文件路径</param>
        /// <returns>序列对象</returns>
        public static object DeserializeFromXML(Type type, string filePath)
        {
            FileStream fs = null;
            try
            {
                fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                XmlSerializer serializer = new XmlSerializer(type);
                return serializer.Deserialize(fs);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (fs != null)
                    fs.Close();
            }
        }

        #endregion

        #region  水印,缩略图

        /// <summary>
        /// 获得当前系统安装的JPEG编码解码器
        /// </summary>
        /// <returns></returns>
        public static ImageCodecInfo GetJPEGCodec()
        {
            if (_isloadjpegcodec == true)
                return _jpegcodec;

            ImageCodecInfo[] codecsList = ImageCodecInfo.GetImageEncoders();
            foreach (ImageCodecInfo codec in codecsList)
            {
                if (codec.MimeType.IndexOf("jpeg") > -1)
                {
                    _jpegcodec = codec;
                    break;
                }

            }
            _isloadjpegcodec = true;
            return _jpegcodec;
        }

        /// <summary>
        /// 生成缩略图
        /// </summary>
        /// <param name="imagePath">图片路径</param>
        /// <param name="thumbPath">缩略图路径</param>
        /// <param name="width">缩略图宽度</param>
        /// <param name="height">缩略图高度</param>
        /// <param name="mode">生成缩略图的方式</param>   
        public static void GenerateThumb(string imagePath, string thumbPath, int width, int height, string mode)
        {
            Image image = Image.FromFile(imagePath);

            string extension = imagePath.Substring(imagePath.LastIndexOf(".")).ToLower();
            ImageFormat imageFormat = null;
            switch (extension)
            {
                case ".jpg":
                case ".jpeg":
                    imageFormat = ImageFormat.Jpeg;
                    break;
                case ".bmp":
                    imageFormat = ImageFormat.Bmp;
                    break;
                case ".png":
                    imageFormat = ImageFormat.Png;
                    break;
                case ".gif":
                    imageFormat = ImageFormat.Gif;
                    break;
                default:
                    imageFormat = ImageFormat.Jpeg;
                    break;
            }

            int toWidth = width > 0 ? width : image.Width;
            int toHeight = height > 0 ? height : image.Height;

            int x = 0;
            int y = 0;
            int ow = image.Width;
            int oh = image.Height;

            switch (mode)
            {
                case "HW"://指定高宽缩放（可能变形）           
                    break;
                case "W"://指定宽，高按比例             
                    toHeight = image.Height * width / image.Width;
                    break;
                case "H"://指定高，宽按比例
                    toWidth = image.Width * height / image.Height;
                    break;
                case "Cut"://指定高宽裁减（不变形）           
                    if ((double)image.Width / (double)image.Height > (double)toWidth / (double)toHeight)
                    {
                        oh = image.Height;
                        ow = image.Height * toWidth / toHeight;
                        y = 0;
                        x = (image.Width - ow) / 2;
                    }
                    else
                    {
                        ow = image.Width;
                        oh = image.Width * height / toWidth;
                        x = 0;
                        y = (image.Height - oh) / 2;
                    }
                    break;
                default:
                    break;
            }

            //新建一个bmp
            Image bitmap = new Bitmap(toWidth, toHeight);

            //新建一个画板
            Graphics g = Graphics.FromImage(bitmap);

            //设置高质量插值法
            g.InterpolationMode = InterpolationMode.High;

            //设置高质量,低速度呈现平滑程度
            g.SmoothingMode = SmoothingMode.HighQuality;

            //清空画布并以透明背景色填充
            g.Clear(Color.Transparent);

            //在指定位置并且按指定大小绘制原图片的指定部分
            g.DrawImage(image,
                        new Rectangle(0, 0, toWidth, toHeight),
                        new Rectangle(x, y, ow, oh),
                        GraphicsUnit.Pixel);

            try
            {
                bitmap.Save(thumbPath, imageFormat);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (g != null)
                    g.Dispose();
                if (bitmap != null)
                    bitmap.Dispose();
                if (image != null)
                    image.Dispose();
            }
        }

        /// <summary>
        /// 生成图片水印
        /// </summary>
        /// <param name="originalPath">源图路径</param>
        /// <param name="watermarkPath">水印图片路径</param>
        /// <param name="targetPath">保存路径</param>
        /// <param name="position">位置</param>
        /// <param name="opacity">透明度</param>
        /// <param name="quality">质量</param>
        public static void GenerateImageWatermark(string originalPath, string watermarkPath, string targetPath, int position, int opacity, int quality)
        {
            Image originalImage = null;
            Image watermarkImage = null;
            //图片属性
            ImageAttributes attributes = null;
            //画板
            Graphics g = null;
            try
            {

                originalImage = Image.FromFile(originalPath);
                watermarkImage = new Bitmap(watermarkPath);

                if (watermarkImage.Height >= originalImage.Height || watermarkImage.Width >= originalImage.Width)
                {
                    originalImage.Save(targetPath);
                    return;
                }

                if (quality < 0 || quality > 100)
                    quality = 80;

                //水印透明度
                float iii;
                if (opacity > 0 && opacity <= 10)
                    iii = (float)(opacity / 10.0F);
                else
                    iii = 0.5F;

                //水印位置
                int x = 0;
                int y = 0;
                switch (position)
                {
                    case 1:
                        x = (int)(originalImage.Width * (float).01);
                        y = (int)(originalImage.Height * (float).01);
                        break;
                    case 2:
                        x = (int)((originalImage.Width * (float).50) - (watermarkImage.Width / 2));
                        y = (int)(originalImage.Height * (float).01);
                        break;
                    case 3:
                        x = (int)((originalImage.Width * (float).99) - (watermarkImage.Width));
                        y = (int)(originalImage.Height * (float).01);
                        break;
                    case 4:
                        x = (int)(originalImage.Width * (float).01);
                        y = (int)((originalImage.Height * (float).50) - (watermarkImage.Height / 2));
                        break;
                    case 5:
                        x = (int)((originalImage.Width * (float).50) - (watermarkImage.Width / 2));
                        y = (int)((originalImage.Height * (float).50) - (watermarkImage.Height / 2));
                        break;
                    case 6:
                        x = (int)((originalImage.Width * (float).99) - (watermarkImage.Width));
                        y = (int)((originalImage.Height * (float).50) - (watermarkImage.Height / 2));
                        break;
                    case 7:
                        x = (int)(originalImage.Width * (float).01);
                        y = (int)((originalImage.Height * (float).99) - watermarkImage.Height);
                        break;
                    case 8:
                        x = (int)((originalImage.Width * (float).50) - (watermarkImage.Width / 2));
                        y = (int)((originalImage.Height * (float).99) - watermarkImage.Height);
                        break;
                    case 9:
                        x = (int)((originalImage.Width * (float).99) - (watermarkImage.Width));
                        y = (int)((originalImage.Height * (float).99) - watermarkImage.Height);
                        break;
                }

                //颜色映射表
                ColorMap colorMap = new ColorMap();
                colorMap.OldColor = Color.FromArgb(255, 0, 255, 0);
                colorMap.NewColor = Color.FromArgb(0, 0, 0, 0);
                ColorMap[] newColorMap = { colorMap };

                //颜色变换矩阵,iii是设置透明度的范围0到1中的单精度类型
                float[][] newColorMatrix ={ 
                                            new float[] {1.0f,  0.0f,  0.0f,  0.0f, 0.0f},
                                            new float[] {0.0f,  1.0f,  0.0f,  0.0f, 0.0f},
                                            new float[] {0.0f,  0.0f,  1.0f,  0.0f, 0.0f},
                                            new float[] {0.0f,  0.0f,  0.0f,  iii, 0.0f},
                                            new float[] {0.0f,  0.0f,  0.0f,  0.0f, 1.0f}
                                           };
                //定义一个 5 x 5 矩阵
                ColorMatrix matrix = new ColorMatrix(newColorMatrix);

                //图片属性
                attributes = new ImageAttributes();
                attributes.SetRemapTable(newColorMap, ColorAdjustType.Bitmap);
                attributes.SetColorMatrix(matrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);

                //画板
                g = Graphics.FromImage(originalImage);
                //绘制水印
                g.DrawImage(watermarkImage, new Rectangle(x, y, watermarkImage.Width, watermarkImage.Height), 0, 0, watermarkImage.Width, watermarkImage.Height, GraphicsUnit.Pixel, attributes);
                //保存图片
                EncoderParameters encoderParams = new EncoderParameters();
                encoderParams.Param[0] = new EncoderParameter(Encoder.Quality, new long[] { quality });
                if (GetJPEGCodec() != null)
                    originalImage.Save(targetPath, _jpegcodec, encoderParams);
                else
                    originalImage.Save(targetPath);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (g != null)
                    g.Dispose();
                if (attributes != null)
                    attributes.Dispose();
                if (watermarkImage != null)
                    watermarkImage.Dispose();
                if (originalImage != null)
                    originalImage.Dispose();
            }
        }

        /// <summary>
        /// 生成文字水印
        /// </summary>
        /// <param name="originalPath">源图路径</param>
        /// <param name="targetPath">保存路径</param>
        /// <param name="text">水印文字</param>
        /// <param name="textSize">文字大小</param>
        /// <param name="textFont">文字字体</param>
        /// <param name="position">位置</param>
        /// <param name="quality">质量</param>
        public static void GenerateTextWatermark(string originalPath, string targetPath, string text, int textSize, string textFont, int position, int quality)
        {
            Image originalImage = null;
            //画板
            Graphics g = null;
            try
            {
                originalImage = Image.FromFile(originalPath);
                //画板
                g = Graphics.FromImage(originalImage);
                if (quality < 0 || quality > 100)
                    quality = 80;

                Font font = new Font(textFont, textSize, FontStyle.Regular, GraphicsUnit.Pixel);
                SizeF sizePair = g.MeasureString(text, font);

                float x = 0;
                float y = 0;

                switch (position)
                {
                    case 1:
                        x = (float)originalImage.Width * (float).01;
                        y = (float)originalImage.Height * (float).01;
                        break;
                    case 2:
                        x = ((float)originalImage.Width * (float).50) - (sizePair.Width / 2);
                        y = (float)originalImage.Height * (float).01;
                        break;
                    case 3:
                        x = ((float)originalImage.Width * (float).99) - sizePair.Width;
                        y = (float)originalImage.Height * (float).01;
                        break;
                    case 4:
                        x = (float)originalImage.Width * (float).01;
                        y = ((float)originalImage.Height * (float).50) - (sizePair.Height / 2);
                        break;
                    case 5:
                        x = ((float)originalImage.Width * (float).50) - (sizePair.Width / 2);
                        y = ((float)originalImage.Height * (float).50) - (sizePair.Height / 2);
                        break;
                    case 6:
                        x = ((float)originalImage.Width * (float).99) - sizePair.Width;
                        y = ((float)originalImage.Height * (float).50) - (sizePair.Height / 2);
                        break;
                    case 7:
                        x = (float)originalImage.Width * (float).01;
                        y = ((float)originalImage.Height * (float).99) - sizePair.Height;
                        break;
                    case 8:
                        x = ((float)originalImage.Width * (float).50) - (sizePair.Width / 2);
                        y = ((float)originalImage.Height * (float).99) - sizePair.Height;
                        break;
                    case 9:
                        x = ((float)originalImage.Width * (float).99) - sizePair.Width;
                        y = ((float)originalImage.Height * (float).99) - sizePair.Height;
                        break;
                }

                g.DrawString(text, font, new SolidBrush(Color.White), x + 1, y + 1);
                g.DrawString(text, font, new SolidBrush(Color.Black), x, y);

                //保存图片
                EncoderParameters encoderParams = new EncoderParameters();
                encoderParams.Param[0] = new EncoderParameter(Encoder.Quality, new long[] { quality });
                if (GetJPEGCodec() != null)
                    originalImage.Save(targetPath, _jpegcodec, encoderParams);
                else
                    originalImage.Save(targetPath);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (g != null)
                    g.Dispose();
                if (originalImage != null)
                    originalImage.Dispose();
            }
        }

        #endregion


        /// <summary>
        /// 生成二维码输出image对象
        /// </summary>
        /// <param name="nr"></param>
        public static Image CreateCodeForImg(string nr)
        {
            QRCodeEncoder qrCodeEncoder = new QRCodeEncoder();
            qrCodeEncoder.QRCodeEncodeMode = QRCodeEncoder.ENCODE_MODE.BYTE;
            qrCodeEncoder.QRCodeScale = 5;
            qrCodeEncoder.QRCodeVersion = 0;
            qrCodeEncoder.QRCodeErrorCorrect = QRCodeEncoder.ERROR_CORRECTION.M;

            Bitmap img = qrCodeEncoder.Encode(nr);
            return img;
        }


        /// <summary>
        /// 生成二维码并保存至文件系统
        /// </summary>
        /// <param name="nr"></param>
        public static string CreateCodeForFile(string nr, bool isMobile = false)
        {
            QRCodeEncoder qrCodeEncoder = new QRCodeEncoder();
            qrCodeEncoder.QRCodeEncodeMode = QRCodeEncoder.ENCODE_MODE.BYTE;
            qrCodeEncoder.QRCodeScale = 5;
            qrCodeEncoder.QRCodeVersion = 0;
            qrCodeEncoder.QRCodeErrorCorrect = QRCodeEncoder.ERROR_CORRECTION.M;

            string filename = "paycode-" + DateTime.Now.ToString("yyyyMMdd hhmmss") + new Random().Next(10000) + ".jpg";
            string filepath = AppDomain.CurrentDomain.BaseDirectory + "/upload" + "/paycode/" + (isMobile ? "/m/" : "/pc/") + filename;

            string RootPath = System.AppDomain.CurrentDomain.BaseDirectory + "/upload/paycode" + (isMobile ? "/m" : "/pc");

            #region 检测日志目录是否存在
            if (!Directory.Exists(RootPath + "/" + ""))
            {
                Directory.CreateDirectory(RootPath + "/" + "");
            }
            lock (ctx)
            {
                if (!System.IO.File.Exists(filepath))
                {
                    Bitmap img = qrCodeEncoder.Encode(nr);
                    img.Save(filepath, System.Drawing.Imaging.ImageFormat.Png);
                    img.Dispose();
                }
                //else
                //    return filename;
            }
            #endregion

            return filename;
        }

        /// <summary>
        /// 生成二维码并保存至文件系统
        /// </summary>
        /// <param name="nr"></param>
        public static string CreateCodeForProduct(string nr, string path, int parentid, string parentname, string pname)
        {
            QRCodeEncoder qrCodeEncoder = new QRCodeEncoder();
            qrCodeEncoder.QRCodeEncodeMode = QRCodeEncoder.ENCODE_MODE.BYTE;
            qrCodeEncoder.QRCodeScale = 5;
            qrCodeEncoder.QRCodeVersion = 0;
            qrCodeEncoder.QRCodeErrorCorrect = QRCodeEncoder.ERROR_CORRECTION.M;

            string filename = parentname + "-" + pname + ".png";
            string filepath = AppDomain.CurrentDomain.BaseDirectory + "/upload" + "/productcode/" + filename;

            string RootPath = System.AppDomain.CurrentDomain.BaseDirectory + "/upload/productcode";

            #region 检测日志目录是否存在
            if (!Directory.Exists(RootPath + "/" + ""))
            {
                Directory.CreateDirectory(RootPath + "/" + "");
            }
            lock (ctx)
            {
                if (!System.IO.File.Exists(filepath))
                {
                    Bitmap img = qrCodeEncoder.Encode(nr);
                    img.Save(filepath, System.Drawing.Imaging.ImageFormat.Png);
                    img.Dispose();
                }
                //else
                //    return filename;
            }
            #endregion

            return filename;
        }

        /// <summary>
        /// 生成二维码
        /// </summary>
        /// <param name="nr"></param>
        public static string CreateCode_Simple(string nr, int uid, string salt, bool isMobile, PartUserInfo partUserInfo)
        {
            QRCodeEncoder qrCodeEncoder = new QRCodeEncoder();
            qrCodeEncoder.QRCodeEncodeMode = QRCodeEncoder.ENCODE_MODE.BYTE;
            qrCodeEncoder.QRCodeScale = 5;
            qrCodeEncoder.QRCodeVersion = 0;
            qrCodeEncoder.QRCodeErrorCorrect = QRCodeEncoder.ERROR_CORRECTION.M;

            string filename = "sharecode-" + uid.ToString() + "-" + salt + ".jpg";
            string filepath = AppDomain.CurrentDomain.BaseDirectory + "/upload" + "/usersharecode/" + (isMobile ? "/m/" : "/pc/") + filename;

            string RootPath = System.AppDomain.CurrentDomain.BaseDirectory + "/upload/usersharecode" + (isMobile ? "/m" : "/pc");

            #region 检测日志目录是否存在
            if (!Directory.Exists(RootPath + "/" + ""))
            {
                Directory.CreateDirectory(RootPath + "/" + "");
            }
            lock (ctx)
            {
                if (!System.IO.File.Exists(filepath))
                {
                    Bitmap img = qrCodeEncoder.Encode(nr);
                    if (!string.IsNullOrEmpty(partUserInfo.Avatar) || !string.IsNullOrEmpty(partUserInfo.OtherLoginId))
                    {
                        Graphics g = Graphics.FromImage(img);
                        g.DrawImage(img, 0, 0, img.Width, img.Height);      //g.DrawImage(imgBack, 0, 0, 相框宽, 相框高);
                        string headImgUrl = string.Empty;
                        if (!string.IsNullOrEmpty(partUserInfo.OtherLoginId))//绑定微信，合成微信的头像
                            headImgUrl = "http://wx.qlogo.cn/mmopen/" + partUserInfo.Avatar;
                        else//未绑定微信，合成自定义的上传头像
                            headImgUrl = string.Format("http://{0}/upload/userheadpic/" + partUserInfo.Avatar, BMAConfig.MallConfig.SiteUrl);
                        System.Net.WebRequest webreq = System.Net.WebRequest.Create(headImgUrl);
                        System.Net.WebResponse webres = webreq.GetResponse();
                        Stream stream = webres.GetResponseStream();
                        Image headImage;
                        headImage = Image.FromStream(stream);
                        stream.Close();
                        //调整头像大小
                        headImage = KiResizeImage(headImage, 75, 75, 0);

                        g.DrawImage(headImage, 75, 75, img.Width, img.Height);
                        GC.Collect();
                    }
                    img.Save(filepath, System.Drawing.Imaging.ImageFormat.Png);
                    img.Dispose();
                }
                //else
                //    return filename;
            }
            #endregion

            return filename;
        }
        /// <summary>
        /// 生成二维码输出image对象
        /// </summary>
        /// <param name="nr"></param>
        public static Image CreateCode_Simple(string nr)
        {
            QRCodeEncoder qrCodeEncoder = new QRCodeEncoder();
            qrCodeEncoder.QRCodeEncodeMode = QRCodeEncoder.ENCODE_MODE.BYTE;
            qrCodeEncoder.QRCodeScale = 5;
            qrCodeEncoder.QRCodeVersion = 0;
            qrCodeEncoder.QRCodeErrorCorrect = QRCodeEncoder.ERROR_CORRECTION.M;

            Bitmap img = qrCodeEncoder.Encode(nr);
            return img;
        }
        /// <summary>
        /// 生成二维码输出image对象--带中间头像
        /// </summary>
        /// <param name="nr"></param>
        public static Image CreateCodeWithHeadImg(string nr, PartUserInfo partUserInfo)
        {
            QRCodeEncoder qrCodeEncoder = new QRCodeEncoder();
            qrCodeEncoder.QRCodeEncodeMode = QRCodeEncoder.ENCODE_MODE.BYTE;
            qrCodeEncoder.QRCodeScale = 5;
            qrCodeEncoder.QRCodeVersion = 0;
            qrCodeEncoder.QRCodeErrorCorrect = QRCodeEncoder.ERROR_CORRECTION.M;

            Bitmap img = qrCodeEncoder.Encode(nr);
            if (!string.IsNullOrEmpty(partUserInfo.Avatar) || !string.IsNullOrEmpty(partUserInfo.OtherLoginId))
            {
                Graphics g = Graphics.FromImage(img);
                g.DrawImage(img, 0, 0, img.Width, img.Height);      //g.DrawImage(imgBack, 0, 0, 相框宽, 相框高);
                string headImgUrl = string.Empty;
                if (!string.IsNullOrEmpty(partUserInfo.OtherLoginId) && !partUserInfo.Avatar.Contains("."))//绑定微信并之后没有修改头像，合成微信的头像
                    headImgUrl = "http://wx.qlogo.cn/mmopen/" + partUserInfo.Avatar;
                else//未绑定微信，或绑定后修改头像，合成自定义的上传头像
                    headImgUrl = string.Format("http://{0}/upload/userheadpic/" + partUserInfo.Avatar, BMAConfig.MallConfig.SiteUrl);
                try
                {
                    System.Net.WebRequest webreq = System.Net.WebRequest.Create(headImgUrl);
                    System.Net.WebResponse webres = webreq.GetResponse();
                    Stream stream = webres.GetResponseStream();
                    Image headImage;
                    headImage = Image.FromStream(stream);
                    stream.Close();
                    //调整头像大小
                    headImage = KiResizeImage(headImage, 65, 65, 0);
                    g.FillRectangle(System.Drawing.Brushes.White, img.Width / 2 - headImage.Width / 2 - 2, img.Height / 2 - headImage.Height / 2 - 2, headImage.Width + 4, headImage.Height + 4);//相片四周刷一层白色边框 
                    g.DrawImage(headImage, img.Width / 2 - headImage.Width / 2, img.Height / 2 - headImage.Height / 2, headImage.Width, headImage.Height);

                    GC.Collect();
                }
                catch
                {
                }
            }
            return img;
        }
        /// <summary>
        /// 生成带有背景的二维码二
        /// </summary>
        /// <returns></returns>
        public static string CreatQRCodeWithLogo(string url, int uid,UserInfo user)
        {
            string BgimgPath = System.AppDomain.CurrentDomain.BaseDirectory + "/upload/bgtemplate/";

            int tempIndex = 1;// new Random().Next(1, 8);
            string BgimgTemp = "bg_temp_" + tempIndex + ".jpg";

            //该方法会锁定图片，不适合图片需要更换情况
            Image bgimg = Image.FromFile(BgimgPath + BgimgTemp);

            //读取本地二维码背景图
            //Image bgimg = ReadImageFile(BgimgPath + BgimgTemp);
            
            Image qrimg = CreateCodeWithLogoImg(url);
            string QRwithBgpath = System.AppDomain.CurrentDomain.BaseDirectory + "/upload/bgqrcode/";
            string filename = "QRcode-" + uid.ToString() + ".jpg";
            string filepath = QRwithBgpath + filename;
            if (!Directory.Exists(QRwithBgpath))
            {
                Directory.CreateDirectory(QRwithBgpath);
            }
            lock (ctx)
            {
                if (!System.IO.File.Exists(filepath))
                {
                    Image QRImgWithBg = CombinImage(bgimg, qrimg,user); //qrimg;
                    QRImgWithBg.Save(QRwithBgpath + filename, System.Drawing.Imaging.ImageFormat.Png);
                    QRImgWithBg.Dispose();
                    bgimg.Dispose();
                }
            }
            return filename;

        }


        /// <summary>
        /// 生成带有背景的二维码二
        /// </summary>
        /// <returns></returns>
        public static string CreatQRCodeWithLogoNew(string url, int uid)
        {
            string BgimgPath = System.AppDomain.CurrentDomain.BaseDirectory + "/upload/bgtemplate/";

            int tempIndex = 1;// new Random().Next(1, 8);
            string BgimgTemp = "bg_temp_" + tempIndex + ".jpg";

            //该方法会锁定图片，不适合图片需要更换情况
            //Image bgimg = Image.FromFile(BgimgPath + BgimgTemp);

            //读取本地二维码背景图
            //Image bgimg = ReadImageFile(BgimgPath + BgimgTemp);

            Image qrimg = CreateCodeWithLogoImg(url);
            string QRwithBgpath = System.AppDomain.CurrentDomain.BaseDirectory + "/upload/bgqrcode/";
            string filename = "QRcode-" + uid.ToString() + ".jpg";
            string filepath = QRwithBgpath + filename;
            if (!Directory.Exists(QRwithBgpath))
            {
                Directory.CreateDirectory(QRwithBgpath);
            }
            lock (ctx)
            {
                if (!System.IO.File.Exists(filepath))
                {
                    Image QRImgWithBg = qrimg;// CombinImage(bgimg, qrimg);
                    QRImgWithBg.Save(QRwithBgpath + filename, System.Drawing.Imaging.ImageFormat.Png);
                    QRImgWithBg.Dispose();
                    //bgimg.Dispose();
                }
            }
            return filename;

        }

        /// <summary>
        /// 生成二维码输出image对象--带中间logo
        /// </summary>
        /// <param name="nr"></param>
        public static Image CreateCodeWithLogoImg(string nr)
        {
            QRCodeEncoder qrCodeEncoder = new QRCodeEncoder();
            qrCodeEncoder.QRCodeEncodeMode = QRCodeEncoder.ENCODE_MODE.BYTE;
            qrCodeEncoder.QRCodeScale = 5;
            qrCodeEncoder.QRCodeVersion = 0;
            qrCodeEncoder.QRCodeErrorCorrect = QRCodeEncoder.ERROR_CORRECTION.M;

            Bitmap img = qrCodeEncoder.Encode(nr);

            Graphics g = Graphics.FromImage(img);
            g.DrawImage(img, 0, 0, img.Width, img.Height);      //g.DrawImage(imgBack, 0, 0, 相框宽, 相框高);
            string headImgUrl = string.Format("http://{0}/usercodelogo.png", BMAConfig.MallConfig.SiteUrl);

            try
            {
                System.Net.WebRequest webreq = System.Net.WebRequest.Create(headImgUrl);
                System.Net.WebResponse webres = webreq.GetResponse();
                Stream stream = webres.GetResponseStream();
                Image headImage;
                headImage = Image.FromStream(stream);
                stream.Close();
                //调整头像大小
                headImage = KiResizeImage(headImage, 32, 32, 0);
                g.FillRectangle(System.Drawing.Brushes.White, img.Width / 2 - headImage.Width / 2 - 2, img.Height / 2 - headImage.Height / 2 - 2, headImage.Width + 4, headImage.Height + 4);//相片四周刷一层白色边框 
                g.DrawImage(headImage, img.Width / 2 - headImage.Width / 2, img.Height / 2 - headImage.Height / 2, headImage.Width, headImage.Height);

                GC.Collect();
            }
            catch
            {
            }
            return img;
        }
        /// <summary>
        /// 生成带有背景的二维码二
        /// </summary>
        /// <returns></returns>
        public static string CreatQRCodeWithBG2(string url, int uid, PartUserInfo partUserInfo)
        {
            string BgimgPath = System.AppDomain.CurrentDomain.BaseDirectory + "/upload/bgtemplate/";

            int tempIndex = 1;// new Random().Next(1, 8);
            string BgimgTemp = "bg_temp_" + tempIndex + ".jpg";

            //该方法会锁定图片，不适合图片需要更换情况
            Image bgimg = Image.FromFile(BgimgPath + BgimgTemp);

            //读取本地二维码背景图
            //Image bgimg = ReadImageFile(BgimgPath + BgimgTemp);

            Image qrimg = CreateCodeWithHeadImg(url, partUserInfo);
            string QRwithBgpath = System.AppDomain.CurrentDomain.BaseDirectory + "/upload/bgqrcode/";
            string filename = "QRcode-" + uid.ToString() + ".jpg";
            string filepath = QRwithBgpath + filename;
            if (!Directory.Exists(QRwithBgpath))
            {
                Directory.CreateDirectory(QRwithBgpath);
            }
            lock (ctx)
            {
                if (!System.IO.File.Exists(filepath))
                {
                    Image QRImgWithBg = CombinImage(bgimg, qrimg,null);
                    QRImgWithBg.Save(QRwithBgpath + filename, System.Drawing.Imaging.ImageFormat.Png);
                    QRImgWithBg.Dispose();
                    bgimg.Dispose();
                }
            }
            return filename;

        }

        /// <summary>
        /// 生成优选带有背景的二维码3
        /// </summary>
        /// <returns></returns>
        public static string CreatQRCodeWithBG3ForYX(string url, int uid, PartUserInfo partUserInfo)
        {
            string BgimgPath = System.AppDomain.CurrentDomain.BaseDirectory + "/upload/bgtemplate/";

            int tempIndex = 1;// new Random().Next(1, 8);
            string BgimgTemp = "yx_bg_temp_" + tempIndex + ".jpg";

            //该方法会锁定图片，不适合图片需要更换情况
            Image bgimg = Image.FromFile(BgimgPath + BgimgTemp);

            //读取本地二维码背景图
            //Image bgimg = ReadImageFile(BgimgPath + BgimgTemp);

            Image qrimg = CreateCodeWithHeadImg(url, partUserInfo);
            string QRwithBgpath = System.AppDomain.CurrentDomain.BaseDirectory + "/upload/yxbgqrcode/";
            string filename = "yx_QRcode-" + uid.ToString() + ".jpg";
            string filepath = QRwithBgpath + filename;
            if (!Directory.Exists(QRwithBgpath))
            {
                Directory.CreateDirectory(QRwithBgpath);
            }
            lock (ctx)
            {
                if (!System.IO.File.Exists(filepath))
                {
                    Image QRImgWithBg = CombinImageForYX(bgimg, qrimg, partUserInfo);
                    QRImgWithBg.Save(QRwithBgpath + filename, System.Drawing.Imaging.ImageFormat.Png);
                    QRImgWithBg.Dispose();
                    bgimg.Dispose();
                }
            }
            return filename;

        }
        /// <summary>
        /// 组合图片  二维码尺寸225*225
        /// </summary>
        /// <param name="imgBack"></param>
        /// <param name="img"></param>
        /// <returns></returns>
        public static Image CombinImageForYX(Image imgBack, Image img, PartUserInfo user)
        {
            //Image img = Image.FromFile(destImg);        //照片图片      
            if (img.Height != 160 || img.Width != 160)
            {
                img = KiResizeImage(img, 160, 160, 0);
            }
            Graphics g = Graphics.FromImage(imgBack);

            g.DrawImage(imgBack, 0, 0, imgBack.Width, imgBack.Height);      //g.DrawImage(imgBack, 0, 0, 相框宽, 相框高);     

            //g.FillRectangle(System.Drawing.Brushes.White, imgBack.Width / 2 - img.Width / 2 - 2, imgBack.Height / 2 - img.Height / 2 - 2, img.Width + 4, img.Height + 4);//相片四周刷一层黑色边框    

            //g.DrawImage(img, 照片与相框的左边距, 照片与相框的上边距, 照片宽, 照片高);    

            //g.DrawImage(img, imgBack.Width / 2 - img.Width / 2, imgBack.Height / 2 - img.Height / 2, img.Width, img.Height);
            //暂时固定偏移为195*493
            //配置文件配置偏移坐标
            //string[] QRcodePoints = string.IsNullOrEmpty(WebConfigurationManager.AppSettings["QRcodePoints"]) ? new string[] { "191", "490" } : WebConfigurationManager.AppSettings["QRcodePoints"].Split(',');

            g.DrawImage(img, 60, 770, img.Width, img.Height);
            string agentTitle = "";
            if (user.AgentType == 5)
                agentTitle = "汇购优选股东";
            if (user.AgentType == 4)
                agentTitle = "战略合伙人";
            if (user.AgentType == 3)
                agentTitle = "H3会员";
            if (user.AgentType == 2)
                agentTitle = "H2会员";
            if (user.AgentType == 1)
                agentTitle = "H1会员";
            if (user.AgentType == 0)
                agentTitle = "无";
            String str = string.Format("会员编号：{0} \r\n代理等级：{1} \r\n扫码或长按识别二维码", string.IsNullOrEmpty(user.UserName) ? user.Mobile : user.UserName, agentTitle);
            Font font = new Font("微软雅黑", 5);
            SolidBrush sbrush = new SolidBrush(Color.Black);
            g.DrawString(str, font, sbrush, new PointF(320, 810));

            GC.Collect();
            return imgBack;
        }
        /// <summary>
        /// 组合图片  二维码尺寸225*225
        /// </summary>
        /// <param name="imgBack"></param>
        /// <param name="img"></param>
        /// <returns></returns>
        public static Image CombinImage(Image imgBack, Image img, UserInfo user)
        {
            //Image img = Image.FromFile(destImg);        //照片图片      
            if (img.Height != 225 || img.Width != 225)
            {
                img = KiResizeImage(img, 450, 450, 0);
            }
            Graphics g = Graphics.FromImage(imgBack);

            g.DrawImage(imgBack, 0, 0, imgBack.Width, imgBack.Height);      //g.DrawImage(imgBack, 0, 0, 相框宽, 相框高);     

            //g.FillRectangle(System.Drawing.Brushes.White, imgBack.Width / 2 - img.Width / 2 - 2, imgBack.Height / 2 - img.Height / 2 - 2, img.Width + 4, img.Height + 4);//相片四周刷一层黑色边框    

            //g.DrawImage(img, 照片与相框的左边距, 照片与相框的上边距, 照片宽, 照片高);    

            //g.DrawImage(img, imgBack.Width / 2 - img.Width / 2, imgBack.Height / 2 - img.Height / 2, img.Width, img.Height);
            //暂时固定偏移为195*493
            //配置文件配置偏移坐标
            string[] QRcodePoints = string.IsNullOrEmpty(WebConfigurationManager.AppSettings["QRcodePoints"]) ? new string[] { "191", "490" } : WebConfigurationManager.AppSettings["QRcodePoints"].Split(',');

            g.DrawImage(img, TypeHelper.StringToInt(QRcodePoints[0]), TypeHelper.StringToInt(QRcodePoints[1]), img.Width, img.Height);
            //姓名
            String str = string.Format("{0}", string.IsNullOrEmpty(user.NickName) ? (string.IsNullOrEmpty(user.RealName) ? "" : user.RealName) : user.NickName);
            Font font = new Font("微软雅黑", 70);
            SolidBrush sbrush = new SolidBrush(Color.DarkOrange);
            g.DrawString(str, font, sbrush, new PointF(700, 165));
            //头像
            string headImgUrl = string.Empty;
            if (user.Avatar.Contains("http://thirdwx.qlogo.cn/mmopen"))//绑定微信并之后没有修改头像，合成微信的头像
                headImgUrl =  user.Avatar;
            else//未绑定微信，或绑定后修改头像，合成自定义的上传头像
                headImgUrl = string.Format("http://{0}/upload/userheadpic/" + user.Avatar, BMAConfig.MallConfig.SiteUrl);
            try
            {
                System.Net.WebRequest webreq = System.Net.WebRequest.Create(headImgUrl);
                System.Net.WebResponse webres = webreq.GetResponse();
                Stream stream = webres.GetResponseStream();
                Image headImage;
                headImage = Image.FromStream(stream);
                stream.Close();
                //调整头像大小
                headImage = KiResizeImage(headImage, 225, 225, 0);
                string[] QRHeadimgPoints = string.IsNullOrEmpty(WebConfigurationManager.AppSettings["QRHeadimgPoints"]) ? new string[] { "300", "165" } : WebConfigurationManager.AppSettings["QRHeadimgPoints"].Split(',');

                //g.FillRectangle(System.Drawing.Brushes.White, img.Width / 2 - headImage.Width / 2 - 2, img.Height / 2 - headImage.Height / 2 - 2, headImage.Width + 4, headImage.Height + 4);//相片四周刷一层白色边框 
                g.DrawImage(headImage, TypeHelper.StringToInt(QRHeadimgPoints[0]), TypeHelper.StringToInt(QRHeadimgPoints[1]), headImage.Width, headImage.Height);

            }
            catch(Exception ex){}

            GC.Collect();
            return imgBack;
        }

        /// <summary>    
        /// Resize图片    
        /// </summary>    
        /// <param name="bmp">原始Bitmap</param>    
        /// <param name="newW">新的宽度</param>    
        /// <param name="newH">新的高度</param>    
        /// <param name="Mode">保留着，暂时未用</param>    
        /// <returns>处理以后的图片</returns>    
        public static Image KiResizeImage(Image bmp, int newW, int newH, int Mode)
        {
            try
            {
                Image b = new Bitmap(newW, newH);
                Graphics g = Graphics.FromImage(b);
                // 插值算法的质量    
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.DrawImage(bmp, new Rectangle(0, 0, newW, newH), new Rectangle(0, 0, bmp.Width, bmp.Height), GraphicsUnit.Pixel);
                g.Dispose();
                return b;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 通过FileStream 来打开文件，这样就可以实现不锁定Image文件，到时可以让多用户同时访问Image文件--报错内存不足
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static Image ReadImageFile(string path)
        {
            FileStream fs = File.OpenRead(path); //OpenRead
            int filelength = 0;
            filelength = (int)fs.Length; //获得文件长度 
            Byte[] image = new Byte[filelength]; //建立一个字节数组 
            fs.Read(image, 0, filelength); //按字节流读取 
            System.Drawing.Image result = System.Drawing.Image.FromStream(fs);
            fs.Close();
            return result;
        }
    }
}
