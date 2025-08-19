(function () {

    window.startVueApp = (selector, options = {}) => {
        Vue.createApp(options).use(Pinia.createPinia()).use(OceanSubsidyComponents).mount(selector);
    };

}());
