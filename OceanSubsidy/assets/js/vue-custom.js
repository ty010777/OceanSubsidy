(function () {

    const rootOptions = {};

    window.setupVueApp = (options) => Object.assign(rootOptions, options);

    window.startVueApp = (selector) => setTimeout(() => Vue.createApp(rootOptions).use(Pinia.createPinia()).use(OceanSubsidyComponents).mount(selector));

}());
