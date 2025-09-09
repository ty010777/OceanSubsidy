export const useGrantStore = defineStore("grant", {
    actions: {
        init(type) {
            this.type = type;
        }
    },
    state: () => ({
        type: null,
        typeList: [
            { code: "SCI", title: "科專" },
            { code: "CUL", title: "文化" },
            { code: "EDC", title: "學校／民間" },
            { code: "CLB", title: "學校社團" },
            { code: "MUL", title: "多元" },
            { code: "LIT", title: "素養" },
            { code: "ACC", title: "無障礙" }
        ]
    })
});
