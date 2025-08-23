<template>
    <div class="block">
        <h5 class="square-title">成果目標與效益</h5>
        <div class="mt-4">
            <div class="table-responsive mt-3 mb-0">
                <table class="table align-middle gray-table">
                    <thead>
                        <tr>
                            <th>可量化成果－指標</th>
                            <th>目標值</th>
                            <th>說明</th>
                            <th width="1" v-if="editable">功能</th>
                        </tr>
                    </thead>
                    <tbody>
                        <tr :key="item" v-for="(item, idx) in filteredBenefits">
                            <td>
                                <input-text :error="errors[`item-${idx}-Title`]" v-model.trim="item.Title"></input-text>
                            </td>
                            <td>
                                <input-text :error="errors[`item-${idx}-Target`]" v-model.trim="item.Target"></input-text>
                            </td>
                            <td>
                                <input-textarea :error="errors[`item-${idx}-Description`]" rows="1" v-model.trim="item.Description"></input-textarea>
                            </td>
                            <td v-if="editable">
                                <div class="d-flex gap-2">
                                    <button class="btn btn-sm btn-teal-dark m-0" @click="remove(item)" type="button" v-if="filteredBenefits.length > 1"><i class="fas fa-trash-alt"></i></button>
                                    <button class="btn btn-sm btn-teal-dark m-0" @click="add" type="button" v-if="idx + 1 === filteredBenefits.length"><i class="fas fa-plus"></i></button>
                                </div>
                            </td>
                        </tr>
                    </tbody>
                </table>
            </div>
            <div class="table-responsive mt-3 mb-0">
                <table class="table align-middle gray-table">
                    <thead>
                        <tr>
                            <th>不可量化成果</th>
                        </tr>
                    </thead>
                    <tbody>
                        <tr>
                            <td>
                                <input-textarea :error="errors.Benefit" rows="4" v-model.trim="project.Benefit"></input-textarea>
                            </td>
                        </tr>
                    </tbody>
                </table>
            </div>
        </div>
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

    const benefits = ref([]);
    const editable = computed(() => isProjectEditable("literacy", project.value.Status, 4));
    const errors = ref({});
    const filteredBenefits = computed(() => benefits.value.filter((item) => !item.Deleted));
    const project = ref({});

    const add = () => benefits.value.push({});

    const remove = (item) => item.Deleted = true;

    const save = (submit) => {
        errors.value = {};

        if (submit && !verify()) {
            nextTick(() => document.querySelector(".invalid")?.scrollIntoView({ behavior: "smooth", block: "center" }));
            return;
        }

        const data = {
            Project: project.value,
            Benefits: benefits.value,
            Submit: submit ? "true" : "false"
        };

        api.literacy("saveBenefit", data).subscribe(() => {
            if (submit) {
                window.location.href = `Attachment.aspx?ID=${props.id}`;
            } else {
                window.location.reload();
            }
        });
    };

    const verify = () => {
        let rules = {
            Benefit: "不可量化成果"
        };

        errors.value = validateData(project.value, rules);

        rules = {
            Title: "指標",
            Target: "目標值",
            Description: "說明"
        };

        filteredBenefits.value.forEach((item, index) => Object.assign(errors.value, validateData(item, rules, `item-${index}-`)));

        return !Object.keys(errors.value).length;
    };

    onMounted(() => {
        rxjs.forkJoin([
            api.literacy("getBenefit", { ID: props.id })
        ]).subscribe((result) => {
            const data = result[0];

            project.value = data.Project;
            benefits.value = data.Benefits;

            if (!benefits.value.length) {
                add();
            }

            useProgressStore().literacy = { step: project.value.FormStep, status: project.value.Status };
        });
    });

    provide("editable", editable);
</script>
