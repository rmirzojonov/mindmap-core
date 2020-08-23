;(function($w){
    'use strict';
    var __name__ = 'Mindmap';

    // Укороченные базовые методы
    var $doc = $w.document;
    var $get = function(id){return $doc.getElementById(id);};
    var $create = function(tag){return $doc.createElement(tag);};
    var $createText = function(n,t){if(n.hasChildNodes()){n.firstChild.nodeValue = t;}else{n.appendChild($doc.createTextNode(t));}};
    var $inner = function(n,t){n.innerHTML = t;};
    var $isElement = function(el){return !!el&&(typeof el==='object')&&(el.nodeType===1)&&(typeof el.style==='object')&&(typeof el.ownerDocument==='object');};
    if(typeof String.prototype.startsWith != 'function'){String.prototype.startsWith=function(p){return this.slice(0,p.length)===p;};}

    // Настройки дерева по умолчанию
    var DEFAULT_OPTIONS = {
        container : '',   
        editable : true, 
        theme : null,
        mode :'full',     
        support_html : true,

        view:{
            hmargin:100,
            vmargin:50,
            line_width:3,
            line_color:'green'
        },
        layout:{
            hspace:30,
            vspace:20,
            pspace:4
        },
        default_event_handle:{
            enable_mousedown_handle:true,
            enable_click_handle:true,
            enable_dblclick_handle:true
        },
        shortcut:{
            enable:true,
            handles:{
            },
            // назначение клавиш на методы
            mapping:{
                addchild   : 9, // Tab
                addbrother : 13, // Enter
                editnode   : 113,// F2
                delnode    : 46, // Delete
                toggle     : 32, // Space
                left       : 37, // Left
                up         : 38, // Up
                right      : 39, // Right
                down       : 40, // Down
            }
        },
    };

    // Основной объект
    var mindmap = function(options){
        mindmap.current = this;
        var opts = {};
        mindmap.util.json.merge(opts, DEFAULT_OPTIONS);
        mindmap.util.json.merge(opts, options);

        this.options = opts;
        this.inited = false;
        this.mind = null;
        this.event_handles = [];
        this.init();
    };

    mindmap.direction = {center:0,right:1};
    mindmap.event_type = {show:1,resize:2,edit:3,select:4};

    // Модель узла дерева
    mindmap.node = function(sId,iIndex,sTopic,oData,bIsRoot,oParent,eDirection){
        this.id = sId;
        this.index = iIndex;
        this.topic = sTopic;
        this.data = oData || {};
        this.isroot = bIsRoot;
        this.parent = oParent;
        this.direction = eDirection;
        this.children = [];
        this._data = {};
    };

    mindmap.node.compare=function(node1,node2){
        var r = 0;
        var i1 = node1.index;
        var i2 = node2.index;
        if(i1>=0 && i2>=0){
            r = i1-i2;
        }else if(i1==-1 && i2==-1){
            r = 0;
        }else if(i1==-1){
            r = 1;
        }else if(i2==-1){
            r = -1;
        }else{
            r = 0;
        }

        return r;
    };

    mindmap.node.prototype = {
        get_location:function(){
            var vd = this._data.view;
            return {
                x:vd.abs_x,
                y:vd.abs_y
            };
        },
        get_size:function(){
            var vd = this._data.view;
            return {
                w:vd.width,
                h:vd.height
            }
        }
    };


    mindmap.mind = function(){
        this.root = null;
        this.selected = null;
        this.nodes = {};
    };

    // Основные методы для манипуляции деревом
    mindmap.mind.prototype = {
        get_node:function(nodeid){
            if(nodeid in this.nodes){
                return this.nodes[nodeid];
            }else{

                return null;
            }
        },

        set_root:function(nodeid, topic, data){
            if(this.root == null){
                this.root = new mindmap.node(nodeid, 0, topic, data, true);
                this._put_node(this.root);
            }
        },

        add_node:function(parent_node, nodeid, topic, data, idx, direction){
            if(!mindmap.util.is_node(parent_node)){
                var the_parent_node = this.get_node(parent_node);
                if(!the_parent_node){
                    return null;
                }else{
                    return this.add_node(the_parent_node, nodeid, topic, data, idx, direction);
                }
            }
            var nodeindex = idx || -1;
            var node = null;
            if(parent_node.isroot){
                var d = mindmap.direction.right;
                /*if(isNaN(direction)){
                    var children = parent_node.children;
                    var children_len = children.length;
                    var r = 0;
                    for(var i=0;i<children_len;i++){if(children[i].direction === mindmap.direction.left){r--;}else{r++;}}
                    d = (children_len > 1 && r > 0) ? mindmap.direction.left : mindmap.direction.right
                }else{
                    d = (direction != mindmap.direction.left) ? mindmap.direction.right : mindmap.direction.left;
                }*/
                node = new mindmap.node(nodeid,nodeindex,topic,data,false,parent_node,d);
            }else{
                node = new mindmap.node(nodeid,nodeindex,topic,data,false,parent_node,parent_node.direction);
            }
            if(this._put_node(node)){
                parent_node.children.push(node);
                this._reindex(parent_node);
            }else{
                node = null;
            }
            return node;
        },

        insert_node_before:function(node_before, nodeid, topic, data){
            if(!mindmap.util.is_node(node_before)){
                var the_node_before = this.get_node(node_before);
                if(!the_node_before){
                    return null;
                }else{
                    return this.insert_node_before(the_node_before, nodeid, topic, data);
                }
            }
            var node_index = node_before.index-0.5;
            return this.add_node(node_before.parent, nodeid, topic, data, node_index);
        },

        get_node_before:function(node){
            if(!mindmap.util.is_node(node)){
                var the_node = this.get_node(node);
                if(!the_node){
                    return null;
                }else{
                    return this.get_node_before(the_node);
                }
            }
            if(node.isroot){return null;}
            var idx = node.index - 2;
            if(idx >= 0){
                return node.parent.children[idx];
            }else{
                return null;
            }
        },

        insert_node_after:function(node_after, nodeid, topic, data){
            if(!mindmap.util.is_node(node_after)){
                var the_node_after = this.get_node(node_before);
                if(!the_node_after){
                    return null;
                }else{
                    return this.insert_node_after(the_node_after, nodeid, topic, data);
                }
            }
            var node_index = node_after.index + 0.5;
            return this.add_node(node_after.parent, nodeid, topic, data, node_index);
        },

        get_node_after:function(node){
            if(!mindmap.util.is_node(node)){
                var the_node = this.get_node(node);
                if(!the_node){
                    return null;
                }else{
                    return this.get_node_after(the_node);
                }
            }
            if(node.isroot){return null;}
            var idx = node.index;
            var brothers = node.parent.children;
            if(brothers.length >= idx){
                return node.parent.children[idx];
            }else{
                return null;
            }
        },

        move_node:function(node, beforeid, parentid, direction){
            if(!mindmap.util.is_node(node)){
                var the_node = this.get_node(node);
                if(!the_node){
                    return null;
                }else{
                    return this.move_node(the_node, beforeid, parentid, direction);
                }
            }
            if(!parentid){
                parentid = node.parent.id;
            }
            return this._move_node(node, beforeid, parentid, direction);
        },

        _flow_node_direction:function(node,direction){
            if(typeof direction === 'undefined'){
                direction = node.direction;
            }else{
                node.direction = direction;
            }
            var len = node.children.length;
            while(len--){
                this._flow_node_direction(node.children[len],direction);
            }
        },

        _move_node_internal:function(node, beforeid){
            if(!!node && !!beforeid){
                if(beforeid == '_last_'){
                    node.index = -1;
                    this._reindex(node.parent);
                }else if(beforeid == '_first_'){
                    node.index = 0;
                    this._reindex(node.parent);
                }else{
                    var node_before = (!!beforeid)?this.get_node(beforeid):null;
                    if(node_before!=null && node_before.parent!=null && node_before.parent.id==node.parent.id){
                        node.index = node_before.index - 0.5;
                        this._reindex(node.parent);
                    }
                }
            }
            return node;
        },

        _move_node:function(node, beforeid, parentid, direction){
            if(!!node && !!parentid){
                if(node.parent.id != parentid){
                    // удалить из списка детей родителя
                    var sibling = node.parent.children;
                    var si = sibling.length;
                    while(si--){
                        if(sibling[si].id == node.id){
                            sibling.splice(si,1);
                            break;
                        }
                    }
                    node.parent = this.get_node(parentid);
                    node.parent.children.push(node);
                }

                if(node.parent.isroot){
                    if(direction == jsMind.direction.left){
                        node.direction = direction;
                    }else{
                        node.direction = mindmap.direction.right;
                    }
                }else{
                    node.direction = node.parent.direction;
                }
                this._move_node_internal(node, beforeid);
                this._flow_node_direction(node);
            }
            return node;
        },

        remove_node:function(node){
            if(!mindmap.util.is_node(node)){
                var the_node = this.get_node(node);
                if(!the_node){
                    return false;
                }else{
                    return this.remove_node(the_node);
                }
            }
            if(!node){
                return false;
            }
            if(node.isroot){
                return false;
            }
            if(this.selected!=null && this.selected.id == node.id){
                this.selected = null;
            }
            // clean all subordinate nodes
            var children = node.children;
            var ci = children.length;
            while(ci--){
                this.remove_node(children[ci]);
            }
            // clean all children
            children.length = 0;
            // remove from parent's children
            var sibling = node.parent.children;
            var si = sibling.length;
            while(si--){
                if(sibling[si].id == node.id){
                    sibling.splice(si,1);
                    break;
                }
            }
            // remove from global nodes
            delete this.nodes[node.id];
            // clean all properties
            for(var k in node){
                delete node[k];
            }
            // remove it's self
            node = null;
            //delete node;
            return true;
        },

        _put_node:function(node){
            if(node.id in this.nodes){
                return false;
            }else{
                this.nodes[node.id] = node;
                return true;
            }
        },

        _reindex:function(node){
            if(node instanceof mindmap.node){
                node.children.sort(mindmap.node.compare);
                for(var i=0;i<node.children.length;i++){
                    node.children[i].index = i+1;
                }
            }
        },
    };

    mindmap.format = {
        node_array:{
            get_mind:function(source){
                var df = mindmap.format.node_array;
                var mind = new mindmap.mind();
                df._parse(mind,source.data);
                return mind;
            },

            get_data:function(mind){
                var df = mindmap.format.node_array;
                var json = {};
                json.format = 'node_array';
                json.data = [];
                df._array(mind,json.data);
                return json;
            },

            _parse:function(mind, node_array){
                var df = mindmap.format.node_array;
                var narray = node_array.slice(0);
                // reverse array for improving looping performance
                narray.reverse();
                var root_id = df._extract_root(mind, narray);
                if(!!root_id){
                    df._extract_subnode(mind, root_id, narray);
                }else{
                }
            },

            _extract_root:function(mind, node_array){
                var df = mindmap.format.node_array;
                var i = node_array.length;
                while(i--){
                    if('isroot' in node_array[i] && node_array[i].isroot){
                        var root_json = node_array[i];
                        var data = df._extract_data(root_json);
                        mind.set_root(root_json.id,root_json.topic,data);
                        node_array.splice(i,1);
                        return root_json.id;
                    }
                }
                return null;
            },

            _extract_subnode:function(mind, parentid, node_array){
                var df = mindmap.format.node_array;
                var i = node_array.length;
                var node_json = null;
                var data = null;
                var extract_count = 0;
                while(i--){
                    node_json = node_array[i];
                    if(node_json.parentid == parentid){
                        data = df._extract_data(node_json);
                        var d = null;
                        var node_direction = node_json.direction;
                        /*if(!!node_direction){
                            d = node_direction == 'left'?mindmap.direction.left:mindmap.direction.right;
                        }*/
                        mind.add_node(parentid, node_json.id, node_json.topic, data, null, d);
                        node_array.splice(i,1);
                        extract_count ++;
                        var sub_extract_count = df._extract_subnode(mind, node_json.id, node_array);
                        if(sub_extract_count > 0){
                            // reset loop index after extract subordinate node
                            i = node_array.length;
                            extract_count += sub_extract_count;
                        }
                    }
                }
                return extract_count;
            },

            _extract_data:function(node_json){
                var data = {};
                for(var k in node_json){
                    if(k == 'id' || k=='topic' || k=='parentid' || k=='isroot' || k=='direction'){
                        continue;
                    }
                    data[k] = node_json[k];
                }
                return data;
            },

            _array:function(mind, node_array){
                var df = mindmap.format.node_array;
                df._array_node(mind.root, node_array);
            },

            _array_node:function(node, node_array){
                var df = mindmap.format.node_array;
                if(!(node instanceof mindmap.node)){return;}
                var o = {
                    id : node.id,
                    topic : node.topic
                };
                if(!!node.parent){
                    o.parentid = node.parent.id;
                }
                if(node.isroot){
                    o.isroot = true;
                }
                if(!!node.parent && node.parent.isroot){
                    //o.direction = node.direction == mindmap.direction.left?'left':'right';
                    o.direction = 'right';
                }
                if(node.data != null){
                    var node_data = node.data;
                    for(var k in node_data){
                        o[k] = node_data[k];
                    }
                }
                node_array.push(o);
                var ci = node.children.length;
                for(var i=0;i<ci;i++){
                    df._array_node(node.children[i], node_array);
                }
            },
        }
    };

    // вспомогательные методы
    mindmap.util = {
        is_node: function(node){
            return !!node && node instanceof mindmap.node;
        },
        
        dom:{
            //target,eventType,handler
            add_event:function(t,e,h){
                if(!!t.addEventListener){
                    t.addEventListener(e,h,false);
                }else{
                    t.attachEvent('on'+e,h);
                }
            }
        },

        canvas:{
            bezierto: function(ctx,x1,y1,x2,y2){
                ctx.beginPath();
                ctx.moveTo(x1,y1);
                ctx.bezierCurveTo(x1+(x2-x1)*2/3,y1,x1,y2,x2,y2);
                ctx.stroke();
            },
            lineto : function(ctx,x1,y1,x2,y2){
                ctx.beginPath();
                ctx.moveTo(x1,y1);
                ctx.lineTo(x2,y2);
                ctx.stroke();
            },
            clear:function(ctx,x,y,w,h){
                ctx.clearRect(x,y,w,h);
            }
        },

        json:{
            json2string:function(json){
                if(!!JSON){
                    try{
                        var json_str = JSON.stringify(json);

                        return json_str;
                    }catch(e){
                        return null;
                    }
                }
            },
            string2json:function(json_str){
                if(!!JSON){
                    try{
                        var json = JSON.parse(json_str);
                       
                        return json;
                    }catch(e){
                        return null;
                    }
                }
            },
            merge:function(b,a){
                for(var o in a){
                    if(o in b){
                        if(typeof b[o] === 'object' &&
                            Object.prototype.toString.call(b[o]).toLowerCase() == '[object object]' &&
                            !b[o].length){
                            mindmap.util.json.merge(b[o], a[o]);
                        }else{
                            b[o] = a[o];
                        }
                    }else{
                        b[o] = a[o];
                    }
                }
                return b;
            }
        },

        // генерация идентификаторов для узлов дерева (Guid)
        uuid:{
            newid:function(){
                return ([1e7] + -1e3 + -4e3 + -8e3 + -1e11).replace(/[018]/g, c =>
                    (c ^ crypto.getRandomValues(new Uint8Array(1))[0] & 15 >> c / 4).toString(16)
                );
            }
        },

        text:{
            is_empty:function(s){
                if(!s){return true;}
                return s.replace(/\s*/,'').length == 0;
            }
        }
    };

    mindmap.prototype={
        init : function(){
            if(this.inited){return;}
            this.inited = true;

            var opts = this.options;

            var opts_layout = {
                mode:opts.mode,
                hspace:opts.layout.hspace,
                vspace:opts.layout.vspace,
                pspace:opts.layout.pspace
            }
            var opts_view = {
                container:opts.container,
                support_html:opts.support_html,
                hmargin:opts.view.hmargin,
                vmargin:opts.view.vmargin,
                line_width:opts.view.line_width,
                line_color:opts.view.line_color
            };

            this.data = new mindmap.data_provider(this);
            this.layout = new mindmap.layout_provider(this, opts_layout);
            this.view = new mindmap.view_provider(this, opts_view);
            this.shortcut = new mindmap.shortcut_provider(this, opts.shortcut);

            this.data.init();
            this.layout.init();
            this.view.init();
            this.shortcut.init();

            this._event_bind();
        },

        enable_edit:function(){
            this.options.editable = true;
        },

        disable_edit:function(){
            this.options.editable = false;
        },

        // call enable_event_handle('dblclick')
        // options are 'mousedown', 'click', 'dblclick'
        enable_event_handle: function(event_handle){
            this.options.default_event_handle['enable_'+event_handle+'_handle'] = true;
        },

        // call disable_event_handle('dblclick')
        // options are 'mousedown', 'click', 'dblclick'
        disable_event_handle: function(event_handle){
            this.options.default_event_handle['enable_'+event_handle+'_handle'] = false;
        },

        get_editable:function(){
            return this.options.editable;
        },

        set_theme:function(theme){
            var theme_old = this.options.theme;
            this.options.theme = (!!theme) ? theme : null;
            if(theme_old != this.options.theme){
                this.view.reset_theme();
                this.view.reset_custom_style();
            }
        },
        _event_bind:function(){
            this.view.add_event(this,'mousedown',this.mousedown_handle);
            this.view.add_event(this,'click',this.click_handle);
            this.view.add_event(this,'dblclick',this.dblclick_handle);
        },

        mousedown_handle:function(e){
            if (!this.options.default_event_handle['enable_mousedown_handle']) {
                return;
            }
            var element = e.target || event.srcElement;
            var nodeid = this.view.get_binded_nodeid(element);
            if(!!nodeid){
                this.select_node(nodeid);
            }else{
                this.select_clear();
            }
        },

        click_handle:function(e){
            if (!this.options.default_event_handle['enable_click_handle']) {
                return;
            }
            
            var element = e.target || event.srcElement;
        },

        dblclick_handle:function(e){
            if (!this.options.default_event_handle['enable_dblclick_handle']) {
                return;
            }
            var element = e.target || event.srcElement;
            var nodeid = this.view.get_binded_nodeid(element);
            if (!!nodeid) {
                var the_node = this.get_node(nodeid);
                if (the_node) {
                    node_dblclick(the_node);
                }
            }
            /*if(this.get_editable()){
                var element = e.target || event.srcElement;
                var nodeid = this.view.get_binded_nodeid(element);
                if(!!nodeid){
                    this.begin_edit(nodeid);
                }
            }*/
            /*e.preventDefault();
            var element = e.target || event.srcElement;
            // прочтите https://stackoverflow.com/questions/27223756/is-element-tagname-always-uppercase
            if(element.tagName.toLowerCase() != "mindmapnode"){ 
                return;
            }*/
        },

        begin_edit:function(node){
            if(!mindmap.util.is_node(node)){
                var the_node = this.get_node(node);
                if(!the_node){
                    return false;
                }else{
                    return this.begin_edit(the_node);
                }
            }
            if(this.get_editable()){
                this.view.edit_node_begin(node);
            }else{
                return;
            }
        },

        end_edit:function(){
            this.view.edit_node_end();

        },

        toggle_node:function(node){
            if(!mindmap.util.is_node(node)){
                var the_node = this.get_node(node);
                if(!the_node){
                    return;
                }else{
                    return this.toggle_node(the_node);
                }
            }
            if(node.isroot){return;}
            this.view.save_location(node);
            this.layout.toggle_node(node);
            this.view.relayout();
            this.view.restore_location(node);
        },
        _reset:function(){
            this.view.reset();
            this.layout.reset();
            this.data.reset();
        },

        _show:function(mind){
            var m = mind || mindmap.format.node_array.example;

            this.mind = this.data.load(m);
            if(!this.mind){
                return;
            }
            this.view.load();
            this.layout.layout();
            this.view.show(true);
            this.invoke_event_handle(mindmap.event_type.show,{data:[mind]});
        },

        show : function(mind){
            this._reset();
            this._show(mind);
        },
        get_data: function(data_format){
            var df = data_format || 'node_tree';
            return this.data.get_data(df);
        },

        get_root:function(){
            return this.mind.root;
        },

        get_node:function(nodeid){
            return this.mind.get_node(nodeid);
        },

        add_node:function(parent_node, nodeid, topic, data){
            if(this.get_editable()){
                var node = this.mind.add_node(parent_node, nodeid, topic, data);
                if(!!node){
                    this.view.add_node(node);
                    this.layout.layout();
                    this.view.show(false);
                    this.view.reset_node_custom_style(node);
                    this.invoke_event_handle(mindmap.event_type.edit,{evt:'add_node',data:[parent_node.id,nodeid,topic,data],node:nodeid});
                }
                return node;
            }else{
                return null;
            }
        },

        insert_node_before:function(node_before, nodeid, topic, data){
            if(this.get_editable()){
                var beforeid = mindmap.util.is_node(node_before) ? node_before.id : node_before;
                var node = this.mind.insert_node_before(node_before, nodeid, topic, data);
                if(!!node){
                    this.view.add_node(node);
                    this.layout.layout();
                    this.view.show(false);
                    this.invoke_event_handle(mindmap.event_type.edit,{evt:'insert_node_before',data:[beforeid,nodeid,topic,data],node:nodeid});
                }
                return node;
            }else{
                return null;
            }
        },

        insert_node_after:function(node_after, nodeid, topic, data){
            if(this.get_editable()){
                var afterid = mindmap.util.is_node(node_after) ? node_after.id : node_after;
                var node = this.mind.insert_node_after(node_after, nodeid, topic, data);
                if(!!node){
                    this.view.add_node(node);
                    this.layout.layout();
                    this.view.show(false);
                    this.invoke_event_handle(mindmap.event_type.edit,{evt:'insert_node_after',data:[afterid,nodeid,topic,data],node:nodeid});
                }
                return node;
            }else{
                return null;
            }
        },

        remove_node:function(node){
            if(!mindmap.util.is_node(node)){
                var the_node = this.get_node(node);
                if(!the_node){
                    return false;
                }else{
                    return this.remove_node(the_node);
                }
            }
            if(this.get_editable()){
                if(node.isroot){
                    return false;
                }
                var nodeid = node.id;
                var parentid = node.parent.id;
                var parent_node = this.get_node(parentid);
                this.view.save_location(parent_node);
                this.view.remove_node(node);
                this.mind.remove_node(node);
                this.layout.layout();
                this.view.show(false);
                this.view.restore_location(parent_node);
                this.invoke_event_handle(mindmap.event_type.edit,{evt:'remove_node',data:[nodeid],node:parentid});
                return true;
            }else{
                return false;
            }
        },

        update_node:function(nodeid, topic){
            if(this.get_editable()){
                if(mindmap.util.text.is_empty(topic)){
                    return;
                }
                var node = this.get_node(nodeid);
                if (!!node) {
                    var v = this;
                    if (node.topic === topic) {
                        v.view.update_node(node);
                        return;
                    }
                    var oldTopic = node.topic;
                    node.topic = topic;
                    v.view.update_node(node);
                    v.layout.layout();
                    v.view.show(false);
                    v.invoke_event_handle(mindmap.event_type.edit, { evt: 'update_node', data: [nodeid, topic], node: nodeid });
                    node_updated(node, function (data) {}, function (data) {
                        node.topic = oldTopic;
                    });
                    
                }
            }else{
                return;
            }
        },

        move_node:function(nodeid, beforeid, parentid, direction){
            if(this.get_editable()){
                var node = this.mind.move_node(nodeid,beforeid,parentid,direction);
                if(!!node){
                    this.view.update_node(node);
                    this.layout.layout();
                    this.view.show(false);
                    this.invoke_event_handle(mindmap.event_type.edit,{evt:'move_node',data:[nodeid,beforeid,parentid,direction],node:nodeid});
                }
            }else{
                return;
            }
        },

        select_node:function(node){
            if(!mindmap.util.is_node(node)){
                var the_node = this.get_node(node);
                if(!the_node){
                    return;
                }else{
                    return this.select_node(the_node);
                }
            }
            this.mind.selected = node;
            this.view.select_node(node);
        },

        get_selected_node:function(){
            if(!!this.mind){
                return this.mind.selected;
            }else{
                return null;
            }
        },

        select_clear:function(){
            if(!!this.mind){
                this.mind.selected = null;
                this.view.select_clear();
            }
        },

        is_node_visible:function(node){
            return this.layout.is_visible(node);
        },

        find_node_before:function(node){
            if(!mindmap.util.is_node(node)){
                var the_node = this.get_node(node);
                if(!the_node){
                    return;
                }else{
                    return this.find_node_before(the_node);
                }
            }
            if(node.isroot){return null;}
            var n = null;
            if(node.parent.isroot){
                var c = node.parent.children;
                var prev = null;
                var ni = null;
                for(var i=0;i<c.length;i++){
                    ni = c[i];
                    if(node.direction === ni.direction){
                        if(node.id === ni.id){
                            n = prev;
                        }
                        prev = ni;
                    }
                }
            }else{
                n = this.mind.get_node_before(node);
            }
            return n;
        },

        find_node_after:function(node){
            if(!mindmap.util.is_node(node)){
                var the_node = this.get_node(node);
                if(!the_node){
                    return;
                }else{
                    return this.find_node_after(the_node);
                }
            }
            if(node.isroot){return null;}
            var n = null;
            if(node.parent.isroot){
                var c = node.parent.children;
                var getthis = false;
                var ni = null;
                for(var i=0;i<c.length;i++){
                    ni = c[i];
                    if(node.direction === ni.direction){
                        if(getthis){
                            n = ni;
                            break;
                        }
                        if(node.id === ni.id){
                            getthis = true;
                        }
                    }
                }
            }else{
                n = this.mind.get_node_after(node);
            }
            return n;
        },

        set_node_color:function(nodeid, bgcolor, fgcolor){
            if(this.get_editable()){
                var node = this.mind.get_node(nodeid);
                if(!!node){
                    if(!!bgcolor){
                        node.data['background-color'] = bgcolor;
                    }
                    if(!!fgcolor){
                        node.data['foreground-color'] = fgcolor;
                    }
                    this.view.reset_node_custom_style(node);
                }
            }else{
                return null;
            }
        },

        set_node_font_style:function(nodeid, size, weight, style){
            if(this.get_editable()){
                var node = this.mind.get_node(nodeid);
                if(!!node){
                    if(!!size){
                        node.data['font-size'] = size;
                    }
                    if(!!weight){
                        node.data['font-weight'] = weight;
                    }
                    if(!!style){
                        node.data['font-style'] = style;
                    }
                    this.view.reset_node_custom_style(node);
                    this.view.update_node(node);
                    this.layout.layout();
                    this.view.show(false);
                }
            }else{
                return null;
            }
        },

        resize:function(){
            this.view.resize();
        },

        // callback(type ,data)
        add_event_listener:function(callback){
            if(typeof callback === 'function'){
                this.event_handles.push(callback);
            }
        },

        invoke_event_handle:function(type, data){
            var j = this;
            $w.setTimeout(function(){
                j._invoke_event_handle(type,data);
            },0);
        },

        _invoke_event_handle:function(type,data){
            var l = this.event_handles.length;
            for(var i=0;i<l;i++){
                this.event_handles[i](type,data);
            }
        }

    };

    mindmap.data_provider = function(mindmap){
        this.mindmap = mindmap;
    };

    mindmap.data_provider.prototype={
        init:function(){
        },

        reset:function(){
        },

        load:function(mind_data){
            var df = null;
            var mind = null;
            if(typeof mind_data === 'object'){
                if(!!mind_data.format){
                    df = mind_data.format;
                }else{
                    df = 'node_tree';
                }
            }
            mind = mindmap.format.node_array.get_mind(mind_data);
            return mind;
        },

        get_data:function(data_format){
            var data = null;
            data = mindmap.format.node_array.get_data(this.mindmap.mind);
            return data;
        },
    };

    mindmap.layout_provider = function(mindmap, options){
        this.opts = options;
        this.mindmap = mindmap;
        this.isside = (this.opts.mode == 'side');
        this.bounds = null;

        this.cache_valid = false;
    };

    mindmap.layout_provider.prototype={
        init:function(){
        },
        reset:function(){
            this.bounds = {n:0,s:0,w:0,e:0};
        },
        layout:function(){
            this.layout_direction();
            this.layout_offset();
        },

        layout_direction:function(){
            this._layout_direction_root();
        },

        _layout_direction_root:function(){
            var node = this.mindmap.mind.root;
            var layout_data = null;
            if('layout' in node._data){
                layout_data = node._data.layout;
            }else{
                layout_data = {};
                node._data.layout = layout_data;
            }
            var children = node.children;
            var children_count = children.length;
            layout_data.direction = mindmap.direction.center;
            layout_data.side_index = 0;
            if(this.isside){
                var i = children_count;
                while(i--){
                    this._layout_direction_side(children[i], mindmap.direction.right, i);
                }
            }else{
                var i = children_count;
                var subnode = null;
                while(i--){
                    subnode = children[i];
                    this._layout_direction_side(subnode, mindmap.direction.right, i);
                    /*if(subnode.direction == mindmap.direction.left){
                        this._layout_direction_side(subnode,mindmap.direction.left, i);
                    }else{
                        this._layout_direction_side(subnode,mindmap.direction.right, i);
                    }*/
                }

            }
        },

        _layout_direction_side:function(node, direction, side_index){
            var layout_data = null;
            if('layout' in node._data){
                layout_data = node._data.layout;
            }else{
                layout_data = {};
                node._data.layout = layout_data;
            }
            var children = node.children;
            var children_count = children.length;

            layout_data.direction = direction;
            layout_data.side_index = side_index;
            var i = children_count;
            while(i--){
                this._layout_direction_side(children[i], direction, i);
            }
        },

        layout_offset:function(){
            var node = this.mindmap.mind.root;
            var layout_data = node._data.layout;
            layout_data.offset_x = 0;
            layout_data.offset_y = 0;
            layout_data.outer_height = 0;
            var children = node.children;
            var i = children.length;
            var left_nodes = [];
            var right_nodes = [];
            var subnode = null;
            while(i--){
                subnode = children[i];
                if(subnode._data.layout.direction == mindmap.direction.right){
                    right_nodes.unshift(subnode);
                }else{
                    left_nodes.unshift(subnode);
                }
            }
            layout_data.left_nodes = left_nodes;
            layout_data.right_nodes = right_nodes;
            layout_data.outer_height_left = this._layout_offset_subnodes(left_nodes);
            layout_data.outer_height_right = this._layout_offset_subnodes(right_nodes);
            this.bounds.e=node._data.view.width/2;
            this.bounds.w=0-this.bounds.e;
            this.bounds.n=0;
            this.bounds.s = Math.max(layout_data.outer_height_left,layout_data.outer_height_right);
        },

        // layout both the x and y axis
        _layout_offset_subnodes:function(nodes){
            var total_height = 0;
            var nodes_count = nodes.length;
            var i = nodes_count;
            var node = null;
            var node_outer_height = 0;
            var layout_data = null;
            var base_y = 0;
            var pd = null; // parent._data
            while(i--){
                node = nodes[i];
                layout_data = node._data.layout;
                if(pd == null){
                    pd = node.parent._data;
                }

                node_outer_height = this._layout_offset_subnodes(node.children);
                node_outer_height = Math.max(node._data.view.height,node_outer_height);

                layout_data.outer_height = node_outer_height;
                layout_data.offset_y = base_y - node_outer_height/2;
                layout_data.offset_x = this.opts.hspace * layout_data.direction + pd.view.width * (pd.layout.direction + layout_data.direction)/2;
                if(!node.parent.isroot){
                    layout_data.offset_x += this.opts.pspace * layout_data.direction;
                }

                base_y = base_y - node_outer_height - this.opts.vspace;
                total_height += node_outer_height;
            }
            if(nodes_count>1){
                total_height += this.opts.vspace * (nodes_count-1);
            }
            i = nodes_count;
            var middle_height = total_height/2;
            while(i--){
                node = nodes[i];
                node._data.layout.offset_y += middle_height;
            }
            return total_height;
        },

        get_node_offset:function(node){
            var layout_data = node._data.layout;
            var offset_cache = null;
            if(('_offset_' in layout_data) && this.cache_valid){
                offset_cache = layout_data._offset_;
            }else{
                offset_cache = {x:-1, y:-1};
                layout_data._offset_ = offset_cache;
            }
            if(offset_cache.x == -1 || offset_cache.y == -1){
                var x = layout_data.offset_x;
                var y = layout_data.offset_y;
                if(!node.isroot){
                    var offset_p = this.get_node_offset(node.parent);
                    x += offset_p.x;
                    y += offset_p.y;
                }
                offset_cache.x = x;
                offset_cache.y = y;
            }
            return offset_cache;
        },

        get_node_point:function(node){
            var view_data = node._data.view;
            var offset_p = this.get_node_offset(node);
            var p = {};
            p.x = offset_p.x + view_data.width*(node._data.layout.direction-1)/2;
            p.y = offset_p.y-view_data.height/2;
            return p;
        },

        get_node_point_in:function(node){
            var p = this.get_node_offset(node);
            return p;
        },

        get_node_point_out:function(node){
            var layout_data = node._data.layout;
            var pout_cache = null;
            if(('_pout_' in layout_data) && this.cache_valid){
                pout_cache = layout_data._pout_;
            }else{
                pout_cache = {x:-1, y:-1};
                layout_data._pout_ = pout_cache;
            }
            if(pout_cache.x == -1 || pout_cache.y == -1){
                if(node.isroot){
                    pout_cache.x = 0;
                    pout_cache.y = 0;
                }else{
                    var view_data = node._data.view;
                    var offset_p = this.get_node_offset(node);
                    pout_cache.x = offset_p.x + (view_data.width+this.opts.pspace)*node._data.layout.direction;
                    pout_cache.y = offset_p.y;
                }
            }
            return pout_cache;
        },

        get_min_size:function(){
            var nodes = this.mindmap.mind.nodes;
            var node = null;
            var pout = null;
            for(var nodeid in nodes){
                node = nodes[nodeid];
                pout = this.get_node_point_out(node);
                if(pout.x > this.bounds.e){this.bounds.e = pout.x;}
                if(pout.x < this.bounds.w){this.bounds.w = pout.x;}
            }
            return {
                w:this.bounds.e - this.bounds.w,
                h:this.bounds.s - this.bounds.n
            }
        },

        toggle_node:function(node){
            if(node.isroot){
                return;
            }
        },

        part_layout:function(node){
            var root = this.mindmap.mind.root;
            if(!!root){
                var root_layout_data = root._data.layout;
                if(node.isroot){
                    root_layout_data.outer_height_right=this._layout_offset_subnodes_height(root_layout_data.right_nodes);
                    root_layout_data.outer_height_left=this._layout_offset_subnodes_height(root_layout_data.left_nodes);
                } else {
                    root_layout_data.outer_height_right = this._layout_offset_subnodes_height(root_layout_data.right_nodes);
                    /*if(node._data.layout.direction == mindmap.direction.right){
                        root_layout_data.outer_height_right=this._layout_offset_subnodes_height(root_layout_data.right_nodes);
                    }else{
                        root_layout_data.outer_height_left=this._layout_offset_subnodes_height(root_layout_data.left_nodes);
                    }*/
                }
                this.bounds.s = Math.max(root_layout_data.outer_height_left,root_layout_data.outer_height_right);
                this.cache_valid = false;
            }else{
            }
        },

    };

    mindmap.view_provider= function(mindmap, options){
        this.opts = options;
        this.mindmap = mindmap;
        this.layout = mindmap.layout;

        this.container = null;
        this.e_panel = null;
        this.e_nodes= null;
        this.e_canvas = null;

        this.canvas_ctx = null;
        this.size = {w:0,h:0};

        this.selected_node = null;
        this.editing_node = null;
    };

    mindmap.view_provider.prototype={
        init:function(){

            this.container = $isElement(this.opts.container) ? this.opts.container : $get(this.opts.container);
            if(!this.container){
                return;
            }
            this.e_panel = $create('div');
            this.e_canvas = $create('canvas');
            this.e_nodes = $create('mindmapnodes');
            this.e_editor = $create('input');

            this.e_panel.appendChild(this.e_canvas);
            this.e_panel.appendChild(this.e_nodes);

            this.e_editor.type = 'text';

            this.actualZoom = 1;
            this.zoomStep = 0.1;
            this.minZoom = 0.5;
            this.maxZoom = 2;

            var v = this;
            mindmap.util.dom.add_event(this.e_editor,'keydown',function(e){
                var evt = e || event;
                if(evt.keyCode == 13){v.edit_node_end();evt.stopPropagation();}
            });
            mindmap.util.dom.add_event(this.e_editor,'blur',function(e){
                v.edit_node_end();
            });
            this.container.appendChild(this.e_panel);

            this.init_canvas();
        },

        add_event:function(obj,event_name,event_handle){
            mindmap.util.dom.add_event(this.e_nodes,event_name,function(e){
                var evt = e || event;
                event_handle.call(obj,evt);
            });
        },

        get_binded_nodeid:function(element){
            if(element == null){
                return null;
            }
            var tagName = element.tagName.toLowerCase();
            if(tagName == 'mindmapnodes' || tagName == 'body' || tagName == 'html'){
                return null;
            }
            if(tagName == 'mindmapnode' || tagName == 'mindmapexpander'){
                return element.getAttribute('nodeid');
            }else{
                return this.get_binded_nodeid(element.parentElement);
            }
        },

        reset:function(){
            this.selected_node = null;
            this.clear_lines();
            this.clear_nodes();
            this.reset_theme();
        },

        reset_theme:function(){
            var theme_name = this.mindmap.options.theme;
            if(!!theme_name){
                this.e_nodes.className = 'theme-' + theme_name;
            }else{
                this.e_nodes.className = '';
            }
        },

        reset_custom_style:function(){
            var nodes = this.mindmap.mind.nodes;
            for(var nodeid in nodes){
                this.reset_node_custom_style(nodes[nodeid]);
            }
        },

        load:function(){
            this.init_nodes();
        },

        expand_size:function(){
            var min_size = this.layout.get_min_size();
            var min_width = min_size.w + this.opts.hmargin*2;
            var min_height = min_size.h + this.opts.vmargin*2;
            var client_w = this.e_panel.clientWidth;
            var client_h = this.e_panel.clientHeight;
            if(client_w < min_width){client_w = min_width;}
            if(client_h < min_height){client_h = min_height;}
            this.size.w = client_w;
            this.size.h = client_h;
        },

        init_canvas:function(){
            var ctx = this.e_canvas.getContext('2d');
            this.canvas_ctx = ctx;
        },

        init_nodes_size:function(node){
            var view_data = node._data.view;
            view_data.width = view_data.element.clientWidth;
            view_data.height = view_data.element.clientHeight;
        },

        init_nodes:function(){
            var nodes = this.mindmap.mind.nodes;
            var doc_frag = $doc.createDocumentFragment();
            for(var nodeid in nodes){
                this.create_node_element(nodes[nodeid],doc_frag);
            }
            this.e_nodes.appendChild(doc_frag);
            for(var nodeid in nodes){
                this.init_nodes_size(nodes[nodeid]);
            }
        },

        add_node:function(node){
            this.create_node_element(node,this.e_nodes);
            this.init_nodes_size(node);
        },

        create_node_element:function(node,parent_node){
            var view_data = null;
            if('view' in node._data){
                view_data = node._data.view;
            }else{
                view_data = {};
                node._data.view = view_data;
            }

            var d = $create('mindmapnode');
            if(node.isroot){
                d.className = 'root';
            }
            if (!!node.topic) {
                if(this.opts.support_html){
                    $inner(d,node.topic);
                }else{
                    $createText(d,node.topic);
                }
            }
            d.setAttribute('nodeid',node.id);
            d.style.visibility='hidden';
            this._reset_node_custom_style(d, node.data);

            parent_node.appendChild(d);
            view_data.element = d;
        },

        remove_node:function(node){
            if(this.selected_node != null && this.selected_node.id == node.id){
                this.selected_node = null;
            }
            if(this.editing_node != null && this.editing_node.id == node.id){
                node._data.view.element.removeChild(this.e_editor);
                this.editing_node = null;
            }
            var children = node.children;
            var i = children.length;
            while(i--){
                this.remove_node(children[i]);
            }
            if(node._data.view){
                var element = node._data.view.element;
                this.e_nodes.removeChild(element);
                node._data.view.element = null;
            }
        },

        update_node:function(node){
            var view_data = node._data.view;
            var element = view_data.element;
            if (!!node.topic) {
                if(this.opts.support_html){
                    $inner(element,node.topic);
                }else{
                    $createText(element,node.topic);
                }
            }
            view_data.width = element.clientWidth;
            view_data.height = element.clientHeight;
        },

        select_node:function(node){
            if(!!this.selected_node){
                this.selected_node._data.view.element.className =
                this.selected_node._data.view.element.className.replace(/\s*selected\b/i,'');
                this.reset_node_custom_style(this.selected_node);
            }
            if(!!node){
                this.selected_node = node;
                node._data.view.element.className += ' selected';
                this.clear_node_custom_style(node);
            }
        },

        select_clear:function(){
            this.select_node(null);
        },

        get_editing_node:function(){
            return this.editing_node;
        },

        is_editing:function(){
            return (!!this.editing_node);
        },

        edit_node_begin:function(node){
            if(!node.topic) {
                return;
            }
            if(this.editing_node != null){
                this.edit_node_end();
            }
            this.editing_node = node;
            var view_data = node._data.view;
            var element = view_data.element;
            var topic = node.topic;
            var ncs = getComputedStyle(element);
            this.e_editor.value = topic;
            this.e_editor.style.color = "Black";
            this.e_editor.style.width = (element.clientWidth-parseInt(ncs.getPropertyValue('padding-left'))-parseInt(ncs.getPropertyValue('padding-right')))+'px';
            element.innerHTML = '';
            element.appendChild(this.e_editor);
            element.style.zIndex = 5;
            this.e_editor.focus();
            this.e_editor.select();
        },

        edit_node_end:function(){
            if(this.editing_node != null){
                var node = this.editing_node;
                this.editing_node = null;
                var view_data = node._data.view;
                var element = view_data.element;
                var topic = this.e_editor.value;
                element.style.zIndex = 'auto';
                element.removeChild(this.e_editor);
                if(mindmap.util.text.is_empty(topic) || node.topic === topic){
                    if(this.opts.support_html){
                        $inner(element,node.topic);
                    }else{
                        $createText(element,node.topic);
                    }
                }else{
                    this.mindmap.update_node(node.id,topic);
                }
            }
        },

        get_view_offset:function(){
            var bounds = this.layout.bounds;
            var _x = (this.size.w - bounds.e - bounds.w)/2;
            var _y = this.size.h / 2;
            return{x:_x, y:_y};
        },

        resize:function(){
            this.e_canvas.width = 1;
            this.e_canvas.height = 1;
            this.e_nodes.style.width = '1px';
            this.e_nodes.style.height = '1px';

            this.expand_size();
            this._show();
        },

        _show:function(){
            this.e_canvas.width = this.size.w;
            this.e_canvas.height = this.size.h;
            this.e_nodes.style.width = this.size.w+'px';
            this.e_nodes.style.height = this.size.h+'px';
            this.show_nodes();
            this.show_lines();
            //this.layout.cache_valid = true;
            this.mindmap.invoke_event_handle(mindmap.event_type.resize,{data:[]});
        },

        zoomIn: function() {
            return this.setZoom(this.actualZoom + this.zoomStep);
        },

        zoomOut: function() {
            return this.setZoom(this.actualZoom - this.zoomStep);
        },

        setZoom: function(zoom) {
            if ((zoom < this.minZoom) || (zoom > this.maxZoom)) {
                return false;
            }
            this.actualZoom = zoom;
            for (var i=0; i < this.e_panel.children.length; i++) {
                this.e_panel.children[i].style.transform = 'scale(' + zoom + ')';
            };
            this.show(true);
            return true;

        },

        _center_root:function(){
            // center root node
            var outer_w = this.e_panel.clientWidth;
            var outer_h = this.e_panel.clientHeight;
            if(this.size.w > outer_w){
                var _offset = this.get_view_offset();
                this.e_panel.scrollLeft = _offset.x - outer_w/2;
            }
            if(this.size.h > outer_h){
                this.e_panel.scrollTop = (this.size.h - outer_h)/2;
            }
        },

        show:function(keep_center){
            this.expand_size();
            this._show();
            if(!!keep_center){
                this._center_root();
            }
        },

        relayout:function(){
            this.expand_size();
            this._show();
        },

        save_location:function(node){
            var vd = node._data.view;
            vd._saved_location={
                x:parseInt(vd.element.style.left)-this.e_panel.scrollLeft,
                y:parseInt(vd.element.style.top)-this.e_panel.scrollTop,
            };
        },

        restore_location:function(node){
            var vd = node._data.view;
            this.e_panel.scrollLeft = parseInt(vd.element.style.left)-vd._saved_location.x;
            this.e_panel.scrollTop = parseInt(vd.element.style.top)-vd._saved_location.y;
        },

        clear_nodes:function(){
            var mind = this.mindmap.mind;
            if(mind == null){
                return;
            }
            var nodes = mind.nodes;
            var node = null;
            for(var nodeid in nodes){
                node = nodes[nodeid];
                node._data.view.element = null;
            }
            this.e_nodes.innerHTML = '';
        },

        // показать узлы
        show_nodes:function(){
            var nodes = this.mindmap.mind.nodes;
            var node = null;
            var node_element = null;
            var p = null;
            var view_data = null;
            var _offset = this.get_view_offset();
            for(var nodeid in nodes){
                node = nodes[nodeid];
                view_data = node._data.view;
                node_element = view_data.element;
                this.reset_node_custom_style(node);
                p = this.layout.get_node_point(node);
                view_data.abs_x = _offset.x + p.x;
                view_data.abs_y = _offset.y + p.y;
                node_element.style.left = (_offset.x+p.x) + 'px';
                node_element.style.top = (_offset.y+p.y) + 'px';
                node_element.style.display = '';
                node_element.style.visibility = 'visible';
            }
        },

        reset_node_custom_style:function(node){
            this._reset_node_custom_style(node._data.view.element, node.data);
        },

        // изменение стилей по умолчанию на новые
        _reset_node_custom_style:function(node_element, node_data){
            if('background-color' in node_data){
                node_element.style.backgroundColor = node_data['background-color'];
            }
            if('foreground-color' in node_data){
                node_element.style.color = node_data['foreground-color'];
            }
            if('width' in node_data){
                node_element.style.width = node_data['width']+'px';
            }
            if('height' in node_data){
                node_element.style.height = node_data['height']+'px';
            }
            if('font-size' in node_data){
                node_element.style.fontSize = node_data['font-size']+'px';
            }
            if('font-weight' in node_data){
                node_element.style.fontWeight = node_data['font-weight'];
            }
            if('font-style' in node_data){
                node_element.style.fontStyle = node_data['font-style'];
            }
            if('background-image' in node_data) {
                var backgroundImage = node_data['background-image'];
                if (backgroundImage.startsWith('data') && node_data['width'] && node_data['height']) {
                    var img = new Image();

                    img.onload = function() {
                        var c = $create('canvas');
                        c.width = node_element.clientWidth;
                        c.height = node_element.clientHeight;
                        var img = this;
                        if(c.getContext) {
                            var ctx = c.getContext('2d');
                            ctx.drawImage(img, 2, 2, node_element.clientWidth, node_element.clientHeight);
                            var scaledImageData = c.toDataURL();
                            node_element.style.backgroundImage='url('+scaledImageData+')';
                        }
                    };
                    img.src = backgroundImage;

                } else {
                    node_element.style.backgroundImage='url('+backgroundImage+')';
                }
                node_element.style.backgroundSize='99%';

                if('background-rotation' in node_data){
                    node_element.style.transform = 'rotate(' + node_data['background-rotation'] + 'deg)';
                }

            }
        },

        clear_node_custom_style:function(node){
            var node_element = node._data.view.element;
            node_element.style.backgroundColor = "";
            node_element.style.color = "";
        },

        clear_lines:function(canvas_ctx){
            var ctx = canvas_ctx || this.canvas_ctx;
            mindmap.util.canvas.clear(ctx,0,0,this.size.w,this.size.h);
        },

        show_lines:function(canvas_ctx){
            this.clear_lines(canvas_ctx);
            var nodes = this.mindmap.mind.nodes;
            var node = null;
            var pin = null;
            var pout = null;
            var _offset = this.get_view_offset();
            for(var nodeid in nodes){
                node = nodes[nodeid];
                if(!!node.isroot){continue;}
                if(('visible' in node._data.layout) && !node._data.layout.visible){continue;}
                pin = this.layout.get_node_point_in(node);
                pout = this.layout.get_node_point_out(node.parent);
                this.draw_line(pout,pin,_offset,canvas_ctx);
            }
        },

        draw_line:function(pin,pout,offset,canvas_ctx){
            var ctx = canvas_ctx || this.canvas_ctx;
            ctx.strokeStyle = this.opts.line_color;
            ctx.lineWidth = this.opts.line_width;
            ctx.lineCap = 'round';

            mindmap.util.canvas.lineto(
                ctx,
                pin.x + offset.x,
                pin.y + offset.y,
                pout.x + offset.x,
                pout.y + offset.y);
            /*mindmap.util.canvas.bezierto(
                ctx,
                pin.x + offset.x,
                pin.y + offset.y,
                pout.x + offset.x,
                pout.y + offset.y);*/
        },
    };

    // shortcut provider
    mindmap.shortcut_provider= function(mindmap, options){
        this.mindmap = mindmap;
        this.opts = options;
        this.mapping = options.mapping;
        this.handles = options.handles;
        this._mapping = {};
    };

    mindmap.shortcut_provider.prototype = {
        init : function(){
            mindmap.util.dom.add_event($doc,'keydown',this.handler.bind(this));

            this.handles['addchild'] = this.handle_addchild;
            this.handles['addbrother'] = this.handle_addbrother;
            this.handles['editnode'] = this.handle_editnode;
            this.handles['delnode'] = this.handle_delnode;
            this.handles['toggle'] = this.handle_toggle;
            this.handles['up'] = this.handle_up;
            this.handles['down'] = this.handle_down;
            this.handles['left'] = this.handle_left;
            this.handles['right'] = this.handle_right;

            for(var handle in this.mapping){
                if(!!this.mapping[handle] && (handle in this.handles)){
                    this._mapping[this.mapping[handle]] = this.handles[handle];
                }
            }
        },

        enable_shortcut : function(){
            this.opts.enable = true;
        },

        disable_shortcut : function(){
            this.opts.enable = false;
        },

        handler : function(e){
            if(this.mindmap.view.is_editing()){return;}
            var evt = e || event;
            if(!this.opts.enable){return true;}
            var kc = evt.keyCode;
            if(kc in this._mapping){
                this._mapping[kc].call(this,this.mindmap,e);
            }
        },

        handle_addchild: function(_mindmap,e){
            var selected_node = _mindmap.get_selected_node();
            if(!!selected_node){
                var nodeid = mindmap.util.uuid.newid();
                var node = _mindmap.add_node(selected_node, nodeid, 'New Node');
                if(!!node){
                    //_mindmap.select_node(nodeid);
                    //_mindmap.begin_edit(nodeid);
                    // узел добавлен
                    node_added(node, function (data) {}, function (data) {
                        _mindmap.remove_node(node);
                    });
                }

            }
        },
        handle_addbrother:function(_mindmap,e){
            var selected_node = _mindmap.get_selected_node();
            if(!!selected_node && !selected_node.isroot){
                var nodeid = mindmap.util.uuid.newid();
                var node = _mindmap.insert_node_after(selected_node, nodeid, 'New Node');
                if(!!node){
                    //_mindmap.select_node(nodeid);
                    //_mindmap.begin_edit(nodeid);
                    // узел добавлен
                    node_added(node, function (data) {}, function (data) {
                        _mindmap.remove_node(node);
                    });
                }
            }
        },
        handle_editnode:function(_mindmap,e){
            var selected_node = _mindmap.get_selected_node();
            if(!!selected_node){
                _mindmap.begin_edit(selected_node);
            }
        },
        handle_delnode:function(_mindmap,e){
            var selected_node = _mindmap.get_selected_node();
            if(!!selected_node && !selected_node.isroot){
                _mindmap.select_node(selected_node.parent);

                // узел удален
                node_deleted(selected_node, function (data) {}, function (data) {
                    _mindmap.remove_node(selected_node);
                });
            }
        },
        handle_toggle:function(_mindmap,e){
            var evt = e || event;
            var selected_node = _mindmap.get_selected_node();
            if(!!selected_node){
                _mindmap.toggle_node(selected_node.id);
                evt.stopPropagation();
                evt.preventDefault();
            }
        },
        handle_up:function(_mindmap,e){
            var evt = e || event;
            var selected_node = _mindmap.get_selected_node();
            if(!!selected_node){
                var up_node = _mindmap.find_node_before(selected_node);
                if(!up_node){
                    var np = _mindmap.find_node_before(selected_node.parent);
                    if(!!np && np.children.length > 0){
                        up_node = np.children[np.children.length-1];
                    }
                }
                if(!!up_node){
                    _mindmap.select_node(up_node);
                }
                evt.stopPropagation();
                evt.preventDefault();
            }
        },

        handle_down:function(_mindmap,e){
            var evt = e || event;
            var selected_node = _mindmap.get_selected_node();
            if(!!selected_node){
                var down_node = _mindmap.find_node_after(selected_node);
                if(!down_node){
                    var np = _mindmap.find_node_after(selected_node.parent);
                    if(!!np && np.children.length > 0){
                        down_node = np.children[0];
                    }
                }
                if(!!down_node){
                    _mindmap.select_node(down_node);
                }
                evt.stopPropagation();
                evt.preventDefault();
            }
        },

        handle_left:function(_mindmap,e){
            this._handle_direction(_mindmap,e,mindmap.direction.left);
        },
        handle_right:function(_mindmap,e){
            this._handle_direction(_mindmap,e,mindmap.direction.right);
        },
        _handle_direction:function(_mindmap,e,d){
            var evt = e || event;
            var selected_node = _mindmap.get_selected_node();
            var node = null;
            if(!!selected_node){
                if(selected_node.isroot){
                    var c = selected_node.children;
                    var children = [];
                    for(var i=0;i<c.length;i++){
                        if(c[i].direction === d){
                            children.push(i)
                        }
                    }
                    node = c[children[Math.floor((children.length-1)/2)]];
                }
                else if(selected_node.direction === d){
                    var children = selected_node.children;
                    var childrencount = children.length;
                    if(childrencount > 0){
                        node = children[Math.floor((childrencount-1)/2)]
                    }
                }else{
                    node = selected_node.parent;
                }
                if(!!node){
                    _mindmap.select_node(node);
                }
                evt.stopPropagation();
                evt.preventDefault();
            }
        },
    };

    mindmap.show = function(options,mind){
        var _mindmap = new mindmap(options);
        _mindmap.show(mind);
        return _mindmap;
    };

    // экспортирование модуля
    if (typeof module !== 'undefined' && typeof exports === 'object') {
        module.exports = mindmap;
    } else if (typeof define === 'function' && (define.amd || define.cmd)) {
        define(function() { return mindmap; });
    } else {
        $w[__name__] = mindmap;
    }
})(typeof window !== 'undefined' ? window : global);