(function () {
    if (map_layers["1CFAA569-648F-49DD-BE98-E32DE9F7C1C6"]) {
        var theLayer = map_layers["1CFAA569-648F-49DD-BE98-E32DE9F7C1C6"].sub["2822E7CD-CC28-40E1-88A7-D08613ABE9F2"];
        if (theLayer) {
            theLayer.rangeQuery = true;
            theLayer.serviceInfo.label = theLayer.serviceInfo.label || {};
            theLayer.serviceInfo.label.template = "{right_user}";
            theLayer.serviceInfo.style =
                {
                    icon: {
                        src: "images/map_legend/water_right1.svg",
                        anchor: [0.5, 0.5],
                        anchorXUnits: 'fraction',
                        anchorYUnits: 'fraction'
                    }
                };

        }
    }
})();