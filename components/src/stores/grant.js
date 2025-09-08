export const useGrantStore = defineStore("grant", {
    actions: {
        init(type) {
            this.type = type;
        }
    },
    state: () => ({
        type: null
    })
});
