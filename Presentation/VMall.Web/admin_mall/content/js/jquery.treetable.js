/* jQuery treeTable Plugin 2.2.3 - http://ludo.cubicphuse.nl/jquery-plugins/treeTable/ */
(function ($) {
    // Helps to make options available to all functions
    // TODO: This gives problems when there are both expandable and non-expandable
    // trees on a page. The options shouldn't be global to all these instances!
    var options;
    var defaultPaddingLeft;

    $.fn.treeTable = function (opts) {
        options = $.extend({}, $.fn.treeTable.defaults, opts);

        return this.each(function () {
            $(this).addClass("treeTable").find("tbody tr").each(function () {
                // Initialize root nodes only if possible
                if (!options.expandable || $(this)[0].className.search("child-of-") == -1) {
                    // To optimize performance of indentation, I retrieve the padding-left
                    // value of the first root node. This way I only have to call +css+ 
                    // once.
                    if (isNaN(defaultPaddingLeft)) {
                        defaultPaddingLeft = parseInt($($(this).children("td")[options.treeColumn]).css('padding-left'), 10);
                    }

                    initialize($(this));
                } else if (options.initialState == "collapsed") {
                    this.style.display = "none"; // Performance! $(this).hide() is slow...
                }
            });
        });
    };

    $.fn.treeTable.defaults = {
        childPrefix: "child-of-",
        clickableNodeNames: false,
        expandable: true,
        indent: 19,
        initialState: "collapsed",
        treeColumn: 0
    };

    // Recursively hide all node's children in a tree
    $.fn.collapse = function () {
        $(this).addClass("collapsed");

        childrenOf($(this)).each(function () {
            if (!$(this).hasClass("collapsed")) {
                $(this).collapse();
            }

            this.style.display = "none"; // Performance! $(this).hide() is slow...
        });

        return this;
    };

    // Recursively show all node's children in a tree
    $.fn.expand = function () {
        $(this).removeClass("collapsed").addClass("expanded");
        var cell = $($(this).children("td")[options.treeColumn]);
        var padding = getPaddingLeft(cell) + options.indent;
        var children = childrenOf($(this));
        children.each(function () {
            $(this).children("td")[options.treeColumn].style.paddingLeft = padding + "px";
            initialize($(this));
            if ($(this).is(".expanded.parent")) {
                $(this).expand();
            }

            // this.style.display = "table-row"; // Unfortunately this is not possible with IE :-(
            $(this).show();
        });

        return this;
    };

    // Add an entire branch to +destination+
    $.fn.appendBranchTo = function (destination) {
        var node = $(this);
        var parent = parentOf(node);

        var ancestorNames = $.map(ancestorsOf($(destination)), function (a) { return a.id; });

        // Conditions:
        // 1: +node+ should not be inserted in a location in a branch if this would
        //    result in +node+ being an ancestor of itself.
        // 2: +node+ should not have a parent OR the destination should not be the
        //    same as +node+'s current parent (this last condition prevents +node+
        //    from being moved to the same location where it already is).
        // 3: +node+ should not be inserted as a child of +node+ itself.
        if ($.inArray(node[0].id, ancestorNames) == -1 && (!parent || (destination.id != parent[0].id)) && destination.id != node[0].id) {
            indent(node, ancestorsOf(node).length * options.indent * -1); // Remove indentation

            if (parent) { node.removeClass(options.childPrefix + parent[0].id); }

            node.addClass(options.childPrefix + destination.id);
            move(node, destination); // Recursively move nodes to new location
            indent(node, ancestorsOf(node).length * options.indent);
        }

        return this;
    };

    // Add reverse() function from JS Arrays
    $.fn.reverse = function () {
        return this.pushStack(this.get().reverse(), arguments);
    };

    // Toggle an entire branch
    $.fn.toggleBranch = function () {
        if ($(this).hasClass("collapsed")) {
            if (childrenOf(this).length == 0) {
                var userid = nodeId(this);
                $.get("/malladmin/user/GetChildren?ParentId=" + userid + "&r=" + Math.random(), function (data) {
                    for (var i = 0; i < data.length; i++) {
                        var tr = $("<tr id='node-" + data[i].Uid + "'  class='child-of-node-" + userid + ((data[i].ChildrenCount > 0) ? " parent" : "") + "' ondblclick='showtree(this)'><td><span class='" + ((data[i].ChildrenCount > 0) ? "folder" : "file") + "'>" + data[i].UserName + "</span></td><td>" + data[i].UserMobile + "</td><td>" + data[i].NickName + "</td><td>" + data[i].RealName + "</td><td>" + data[i].AgentRank + "</td><td>" + data[i].AgentSource + "</td><td>" + data[i].RegisterTime + "</td><td><a class='editOperate' href='/malladmin/user/List?userName=" + data[i].UserName + "&mobile=" + data[i].UserMobile + "'>查看详情</a></td></tr>");
                        //<td>" + ((data[i].ChildrenCount == null) ? "0" : data[i].ChildrenCount) + "</td>
                        $("#node-" + userid).after(tr);
                        tr.mousedown(function () {
                            $("tr.selected").removeClass("selected"); // Deselect currently selected rows
                            $(this).addClass("selected");
                        });

                        //鼠标过行的变色效果
                        tr.mouseover(function () {
                            $(this).addClass("over");
                        });

                        tr.mouseout(function () {
                            $(this).removeClass("over");
                        });
                    }
                    $("#node-" + userid).expand();
                }, "json");
            }
            else {
                $(this).expand();
            }
        } else {
            $(this).removeClass("expanded").collapse();
        }

        return this;
    };

    // === Private functions

    function ancestorsOf(node) {
        var ancestors = [];
        while (node = parentOf(node)) {
            ancestors[ancestors.length] = node[0];
        }
        return ancestors;
    };

    function childrenOf(node) {
        return $("table.treeTable tbody tr." + options.childPrefix + node[0].id);
    };

    function getPaddingLeft(node) {
        var paddingLeft = parseInt(node[0].style.paddingLeft, 10);
        return (isNaN(paddingLeft)) ? defaultPaddingLeft : paddingLeft;
    }

    function indent(node, value) {
        var cell = $(node.children("td")[options.treeColumn]);
        cell[0].style.paddingLeft = getPaddingLeft(cell) + value + "px";

        childrenOf(node).each(function () {
            indent($(this), value);
        });
    };

    function initialize(node) {
        if (!node.hasClass("initialized")) {
            node.addClass("initialized");

            var childNodes = childrenOf(node);

            if (!node.hasClass("parent") && childNodes.length > 0) {
                node.addClass("parent");
            }

            if (node.hasClass("parent")) {
                var cell = $(node.children("td")[options.treeColumn]);
                var padding = getPaddingLeft(cell) + options.indent;

                childNodes.each(function () {
                    $(this).children("td")[options.treeColumn].style.paddingLeft = padding + "px";
                });

                if (options.expandable) {
                    cell.prepend('<span style="margin-left: -' + options.indent + 'px; padding-left: ' + options.indent + 'px" class="expander"></span>');
                    $(cell[0].firstChild).click(function () { node.toggleBranch(); });

                    if (options.clickableNodeNames) {
                        cell[0].style.cursor = "pointer";
                        $(cell).click(function (e) {
                            // Don't double-toggle if the click is on the existing expander icon
                            if (e.target.className != 'expander') {
                                node.toggleBranch();
                            }
                        });
                    }

                    // Check for a class set explicitly by the user, otherwise set the default class
                    if (!(node.hasClass("expanded") || node.hasClass("collapsed"))) {
                        node.addClass(options.initialState);
                    }

                    if (node.hasClass("expanded")) {
                        node.expand();
                    }
                }
            }
        }
    };

    function move(node, destination) {
        node.insertAfter(destination);
        childrenOf(node).reverse().each(function () { move($(this), node[0]); });
    };

    function parentOf(node) {
        var classNames = node[0].className.split(' ');

        for (key in classNames) {
            if (classNames[key].match("child-of-")) {
                return $("#" + classNames[key].substring(9));
            }
        }
    };
    function nodeId(node) {
        return node[0].id.substring(5);
    }
})(jQuery);