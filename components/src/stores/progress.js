export const useProgressStore = defineStore("progress", {
    actions: {
        init(type, data) {
            this[type] = {
                id: data.ProjectID,
                type: data.SubsidyPlanType?.split(" ")[0],
                name: data.ProjectName,
                step: data.FormStep,
                status: data.Status,
                progress: data.ProgressStatus,
                organizer: data.Organizer,
                organizerName: data.OrganizerName
            };
        }
    },
    state: () => ({
        accessibility: {},
        culture: {},
        education: {},
        literacy: {},
        multiple: {}
    })
});
