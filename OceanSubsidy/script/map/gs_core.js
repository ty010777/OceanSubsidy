gs = {};
gs.Core = {};

gs.Core.package = function (packageName) {
    var tokens = packageName.split(".");
    var container = window;
    for (var i = 0; i < tokens.length; i++) {
        container[tokens[i]] = container[tokens[i]] || {};
        container = container[tokens[i]];
    }
    return container;
};