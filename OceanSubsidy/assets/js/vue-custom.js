(function () {

    const rootOptions = {};

    window.setupVueApp = (options) => Object.assign(rootOptions, options);

    window.startVueApp = (selector) => setTimeout(() => {
        OceanSubsidyComponents.api.setBaseUrl(window.AppRootPath || "");

        Vue.createApp(rootOptions).use(Pinia.createPinia()).use(OceanSubsidyComponents).mount(selector);
    });

}());
