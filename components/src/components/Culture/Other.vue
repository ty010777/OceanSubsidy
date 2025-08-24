<template>
    <div class="block">
        <h5 class="square-title">
            提案單位近三年執行相關計畫之成效及與本計畫之關聯性與差異性
        </h5>
        <p class="text-pink lh-base mt-3" v-if="editable">
            若無相關計畫可不填寫
        </p>
        <div class="mt-3" :key="item" v-for="(item, idx) in filteredProjects">
            <div class="border">
                <div class="bg-light-teal p-3 text-gray d-flex align-items-center gap-3">
                    <div class="d-flex gap-2 align-items-center flex-grow-1">
                        <label class="text-nowrap"><required-label>相關計畫名稱</required-label></label>
                        <div class="flex-grow-1">
                            <input-text :error="errors[`project-${idx}-Title`]" v-model.trim="item.Title"></input-text>
                        </div>
                    </div>
                    <div class="d-flex gap-2">
                        <a class="link-teal" @click.prevent="remove(item)" href="#" v-if="editable">刪除</a>
                        <a aria-expanded="true" class="link-teal" data-bs-toggle="collapse" :href="`#collapse-${idx}`" role="button">收合</a>
                    </div>
                </div>
                <div class="collapse show" :id="`collapse-${idx}`">
                    <table class="table align-middle gray-table side-table mb-0">
                        <tbody>
                            <tr>
                                <th><required-label>執行年度</required-label></th>
                                <td>
                                    <input-integer class="text-start w-auto" :error="errors[`project-${idx}-Year`]" v-model="item.Year"></input-integer>
                                </td>
                            </tr>
                            <tr>
                                <th><required-label>執行單位</required-label></th>
                                <td>
                                    <input-text :error="errors[`project-${idx}-OrgName`]" v-model.trim="item.OrgName"></input-text>
                                </td>
                            </tr>
                            <tr>
                                <th><required-label>計畫經費(元)</required-label></th>
                                <td>
                                    <input-integer class="w-auto" :error="errors[`project-${idx}-Amount`]" v-model="item.Amount"></input-integer>
                                </td>
                            </tr>
                            <tr>
                                <th><required-label>內容摘述</required-label></th>
                                <td>
                                    <input-textarea :error="errors[`project-${idx}-Description`]" rows="4" v-model.trim="item.Description"></input-textarea>
                                </td>
                            </tr>
                            <tr>
                                <th><required-label>執行效益</required-label></th>
                                <td>
                                    <input-textarea :error="errors[`project-${idx}-Benefit`]" rows="4" v-model.trim="item.Benefit"></input-textarea>
                                </td>
                            </tr>
                        </tbody>
                    </table>
                </div>
            </div>
        </div>
        <button class="btn btn-teal-dark mt-3" @click="add" type="button" v-if="editable"><i class="fas fa-plus"></i>新增計畫目標</button>
    </div>
    <div class="block-bottom bg-light-teal" v-if="editable">
        <button class="btn btn-outline-teal me-3" @click="save()" type="button">暫存</button>
        <button class="btn btn-teal" @click="save(true)" type="button"><i class="fas fa-check"></i>完成本頁，下一步</button>
    </div>
</template>

<script setup>
    const props = defineProps({
        id: { type: Number }
    });

    const editable = computed(() => isProjectEditable("culture", project.value.Status, 4));
    const errors = ref({});
    const project = ref({});
    const projects = ref([]);

    const filteredProjects = computed(() => projects.value.filter((item) => !item.Deleted));

    const add = () => projects.value.push({});

    const remove = (item) => item.Deleted = true;

    const save = (submit) => {
        errors.value = {};

        if (submit && !verify()) {
            nextTick(() => document.querySelector(".invalid")?.scrollIntoView({ behavior: "smooth", block: "center" }));
            return;
        }

        const data = {
            ID: props.id,
            Projects: projects.value,
            Submit: submit ? "true" : "false"
        };

        api.culture("saveRelatedProject", data).subscribe(() => {
            if (submit) {
                window.location.href = `Attachment.aspx?ID=${props.id}`;
            } else {
                window.location.reload();
            }
        });
    };

    const verify = () => {
        errors.value = {};

        const rules = {
            Title: "相關計畫名稱",
            Year: "執行年度",
            OrgName: "執行單位",
            Amount: { presence: { allowEmpty: false, message: `^請輸入計畫經費` }, numericality: { greaterThan: 0, message: "^計畫經費不可為 0" } },
            Description: "內容摘述",
            Benefit: "執行效益"
        };

        filteredProjects.value.forEach((project, index) => Object.assign(errors.value, validateData(project, rules, `project-${index}-`)));

        return !Object.keys(errors.value).length;
    };

    onMounted(() => {
        rxjs.forkJoin([
            api.culture("getRelatedProject", { ID: props.id })
        ]).subscribe((result) => {
            const data = result[0];

            project.value = data.Project;
            projects.value = data.Projects;

            useProgressStore().culture = { step: project.value.FormStep, status: project.value.Status, organizer: project.value.Organizer, organizerName: project.value.OrganizerName };
        });
    });

    provide("editable", editable);
</script>
