<template>
    <div class="block">
        <h5 class="square-title">上傳附件</h5>
        <p class="text-pink lh-base mt-4" v-if="editable">
            請下載附件範本，填寫資料及公文用印後上傳（僅支援PDF格式上傳，每個檔案10MB以內）<br>
            計畫書請自行留存送審版電子檔，待決審核定經費後請提交修正計畫書以供核定。
        </p>
        <div class="mt-4">
            <table class="table align-middle gray-table">
                <thead class="text-center">
                    <tr>
                        <th width="60">附件編號</th>
                        <th>附件名稱</th>
                        <th width="180" v-if="editable">狀態</th>
                        <th width="350">上傳附件</th>
                    </tr>
                </thead>
                <tbody>
                    <tr :key="item" v-for="(item, idx) in docs">
                        <td class="text-center">
                            {{ idx + 1 }}
                        </td>
                        <td>
                            <div>
                                <span v-if="item.Optional">{{ item.Title }}</span>
                                <required-label v-else>{{ item.Title }}</required-label>
                            </div>
                            <div class="mt-3 small" v-html="item.Comment" v-if="editable && item.Comment"></div>
                             <a class="btn btn-sm btn-teal-dark rounded-pill mt-2" download :href="item.Template" style="width:140px" type="button" v-if="editable && item.Template">
                                <i class="fas fa-file-download me-1"></i>範本下載
                            </a>
                        </td>
                        <td class="text-center" v-if="editable">
                            <span v-if="item.Uploaded">已上傳</span>
                            <span class="text-pink" v-else>尚未上傳</span>
                        </td>
                        <td>
                            <input-file accept=".pdf" @file="(file) => add(item, file)" v-if="editable && (item.Multiple || !item.Uploaded)"></input-file>
                            <div class="tag-group mt-2 gap-1" v-if="item.Uploaded">
                                <template :key="file" v-for="(file) in item.Files">
                                    <span class="tag tag-green-light" v-if="!file.Deleted">
                                        <a class="tag-link my-1" :download="file.Name" :href="download(file.Path)">{{ file.Name }}</a>
                                        <button class="tag-btn" @click="file.Deleted = true" type="button" v-if="editable">
                                            <i class="fa-solid fa-circle-xmark"></i>
                                        </button>
                                    </span>
                                </template>
                            </div>
                        </td>
                    </tr>
                </tbody>
            </table>
        </div>
        <div class="text-center mt-4" v-if="project.ApplyTime">申請送件時間：<tw-date-time :value="project.ApplyTime"></tw-date-time></div>
        <template v-if="changeForm">
            <h5 class="square-title mt-5">變更說明</h5>
            <div class="mt-4">
                <table class="table align-middle gray-table side-table">
                    <tbody>
                        <tr>
                            <th><required-label>變更前</required-label></th>
                            <td>
                                <input-textarea :error="errors.Form5Before" rows="4" v-model.trim="changeForm.Form5Before"></input-textarea>
                            </td>
                        </tr>
                        <tr>
                            <th><required-label>變更後</required-label></th>
                            <td>
                                <input-textarea :error="errors.Form5After" rows="4" v-model.trim="changeForm.Form5After"></input-textarea>
                            </td>
                        </tr>
                    </tbody>
                </table>
            </div>
        </template>
    </div>
    <div class="block-bottom bg-light-teal" v-if="editable">
        <button class="btn btn-outline-teal me-3" @click="save()" type="button">暫存</button>
        <button class="btn btn-teal" @click="save(true)" :disabled="disabled" type="button"><i class="fas fa-check"></i>全部完成，提送申請</button>
    </div>
</template>

<script setup>
    const props = defineProps({
        apply: { default: false, type: Boolean },
        id: { type: [Number, String] }
    });

    const docs = ref([
        { Type: 1, Title: "申請表", Template: `../../Service/OFS/DownloadTemplateCUL.ashx?type=1&id=${props.id}`, Files: [] },
        { Type: 2, Title: "申請捐（補）助計畫書", Template: `../../Service/OFS/DownloadTemplateCUL.ashx?type=2&id=${props.id}`, Files: [] },
        { Type: 3, Title: "執行承諾書", Template: `../../Service/OFS/DownloadTemplateCUL.ashx?type=3&id=${props.id}`, Files: [] },
        { Type: 4, Title: "未違反公職人員利益衝突迴避法切結書及事前揭露表", Template: `../../Service/OFS/DownloadTemplateCUL.ashx?type=4&id=${props.id}`, Files: [], Excludes: ["1"] },
        { Type: 5, Title: "附錄", Comment: "(與本計畫相關之補充資料、如立案證明、配合提案計畫邀集相關合作單位召開討論之會議紀錄等；若有合作對象，請附合作對象同意書)", Files: [], Multiple: true, Optional: true }
    ]);

    const changeForm = ref();
    const disabled = computed(() => docs.value.some((doc) => !doc.Optional && !doc.Files.filter((file) => !file.Deleted).length));
    const editable = computed(() => isProjectEditable("culture", project.value.Status, 5));
    const errors = ref({});
    const project = ref({});

    const add = (item, file) => {
        if (file.Type === "application/pdf") {
            item.Files.push({
                Type: item.Type,
                Path: file.Path,
                Name: project.value.ProjectID + "_" + (item.Multiple ? file.Name : `${item.Title}.pdf`)
            });
        }
    };

    const download = api.download;

    const emit = defineEmits(["next"]);

    const load = () => {
        rxjs.forkJoin([
            api.culture("getAttachment", { Apply: props.apply ? "true" : "false", ID: props.id })
        ]).subscribe((result) => {
            const data = result[0];

            project.value = data.Project;
            docs.value = docs.value.filter((doc) => !doc.Excludes?.includes(project.value.OrgCategory));

            if (project.value.Status >= 42) {
                docs.value.push({ Type: 6, Title: "著作權授權同意書", Template: "../../Template/CUL/5.海洋委員會補助計畫著作權授權同意書.docx", Files: [] });
            }

            docs.value.forEach((doc) => {
                doc.Files = data.Attachments.filter((file) => file.Type === doc.Type);
                doc.Uploaded = computed(() => doc.Files.filter((file) => !file.Deleted).length);
            });

            useProgressStore().init("culture", project.value);

            changeForm.value = project.value.changeApply;
            project.value.changeApply = undefined;
        });
    };

    const save = (submit) => {
        errors.value = {};

        if (submit && !verify()) {
            nextTick(() => document.querySelector(".invalid")?.scrollIntoView({ behavior: "smooth", block: "center" }));
            return;
        }

        const data = {
            ID: props.id,
            Attachments: docs.value.reduce((list, item) => list.concat(item.Files), []),
            Before: changeForm.value?.Form5Before,
            After: changeForm.value?.Form5After,
            Submit: submit ? "true" : "false"
        };

        api.culture("saveAttachment", data).subscribe((res) => {
            if (res) {
                if (submit) {
                    emit("next", load);
                } else {
                    load();
                }
            }
        });
    };

    const verify = () => {
        if (changeForm.value) {
            const rules = {
                Form5Before: "變更前說明",
                Form5After: "變更後說明"
            };

            Object.assign(errors.value, validateData(changeForm.value, rules));
        }

        return !Object.keys(errors.value).length;
    };

    onMounted(load);

    provide("editable", editable);
</script>
