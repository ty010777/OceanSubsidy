export const useProgressStore = defineStore("progress", {
    actions: {
        init(type, data, report) {
            this[type] = {
                id: data.ProjectID,
                type: data.SubsidyPlanType?.split(" ")[0],
                name: data.ProjectName,
                step: data.FormStep,
                status: data.Status,
                progress: data.ProgressStatus,
                changeStatus: data.changeApply?.Status,
                reportStatus: report?.Status,
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
