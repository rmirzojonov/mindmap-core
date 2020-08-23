var modal = document.getElementById('myModal');
window.onclick = function (event) {
    if (event.target === modal) {
        modal.style.display = "none";
    }
}
var span = document.getElementById("modal-close");
span.onclick = function () {
    modal.style.display = "none";
}

var data;
var mindmap;
var tree_id = document.getElementById("treeid").value;
var request_url = "/api/Nodes/";

// объект для работы с ajax запросами
ajax =  {
    _xhr: function() {
        var xhr = null;
        if (window.XMLHttpRequest) {
            xhr = new XMLHttpRequest();
        } else {
            try {
                xhr = new ActiveXObject('Microsoft.XMLHTTP');
            } catch (e) { }
        }
        return xhr;
    },
    _eurl: function(url) {
        return encodeURIComponent(url);
    },
    get: function (url, success_callback, failure_callback) {
        $.ajax({
            url: url,
            type: "GET",
            success: function (data) {
                if (success_callback) {
                    success_callback(data);
                }
            },
            error: function (data) {
                if (failure_callback) {
                    failure_callback(data);
                }
            }
        });
    },
    post: function (url, param, success_callback, failure_callback) {
        $.ajax({
            url: url,
            type: "POST",
            contentType: "application/json;charset=utf-8",
            data: JSON.stringify(param),
            success: function (data) {
                if (success_callback) {
                    success_callback(data);
                }
            },
            error: function (data) {
                if (failure_callback) {
                    failure_callback(data);
                }
            }
        });
    },
    delete: function (url, success_callback, failure_callback) {
        $.ajax({
            url: url,
            type: "DELETE",
            success: function (data) {
                if (success_callback) {
                    success_callback(data);
                }
            },
            error: function (data) {
                if (failure_callback) {
                    failure_callback(data);
                }
            }
        });
    },
    put: function (url, param, success_callback, failure_callback) {
        $.ajax({
            url: url,
            type: "PUT",
            contentType: "application/json;charset=utf-8",
            data: JSON.stringify(param),
            success: function (data) {
                if (success_callback) {
                    success_callback(data);
                }
            },
            error: function (data) {
                if (failure_callback) {
                    failure_callback(data);
                }
            }
        });
    }
};

// вызывается когда узел дерева был нажат дважды
function node_dblclick(node) {
    modal.style.display = "block";
    var mtext = document.getElementById("modal-text");
    mtext.innerHTML = node.topic;
}

// метод отправляющий запрос на добавление узла в бд 
function node_added(node, success_callback, failure_callback) {
    var dto = { "id": node.id, "treeid": tree_id, "parentid": node.parent.id, "topic": node.topic };
    ajax.post(request_url, dto, success_callback(data), failure_callback(data));
}

// метод отправляющий запрос на изменение узла в бд
function node_updated(node, success_callback, failure_callback) {
    var dto = { "id": node.id, "treeid": tree_id, "parentid": node.parent.id, "topic": node.topic };
    ajax.put(request_url + node.id, dto, success_callback(data), failure_callback(data));
}

// метод отправляющий запрос на удаление узла и его потомков из бд 
function node_deleted(node, success_callback, failure_callback) {
    // Отправка запроса на удаление узла из базы данных
    ajax.delete(request_url + node.id, success_callback(data), failure_callback(data));
}

