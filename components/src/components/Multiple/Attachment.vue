<template>
    <div class="block">
        <h5 class="square-title">請下載範本填寫用印並上傳</h5>
        <p class="text-pink lh-base mt-4" v-if="editable">
            請下載附件範本，填寫資料及公文用印後上傳（僅支援PDF格式上傳，每個檔案10MB以內）<br />
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
                    <tr :key="item" v-for="(item,idx) in docs">
                        <td class="text-center">
                            {{ idx + 1 }}
                        </td>
                        <td>
                            <div>
                                <span v-if="item.Optional">{{ item.Title }}</span>
                                <required-label v-else>{{ item.Title }}</required-label>
                            </div>
                            <div class="mt-3 small" v-html="item.Comment" v-if="editable && item.Comment"></div>
                            <button class="btn btn-sm btn-teal-dark rounded-pill mt-2" type="button" v-if="editable && item.Template">
                                <i class="fas fa-file-download me-1"></i>範本下載
                            </button>
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
    </div>
    <div class="block-bottom bg-light-teal" v-if="editable">
        <button class="btn btn-outline-teal me-3" @click="save()" type="button">暫存</button>
        <button class="btn btn-teal" @click="save(true)" :disabled="disabled" type="button"><i class="fas fa-check"></i>全部完成，提送申請</button>
    </div>
</template>

<script setup>
    const props = defineProps({
        id: { type: Number }
    });

    const docs = ref([
        { Type: 1, Title: "申請表", Template: true, Files: [] },
        { Type: 2, Title: "計畫書", Template: true, Files: [] },
        { Type: 3, Title: "執行承諾書", Template: true, Files: [] },
        { Type: 4, Title: "未違反公職人員利益衝突迴避法切結書及事前揭露表", Template: true, Files: [] },
        { Type: 5, Title: "其他佐證資料", Comment: "提案計畫倘有結合其他合作單位、團體或相關機關(構)，請提供合作意向書或相關參考文件尤佳，無則免附", Files: [], Multiple: true, Optional: true }
    ]);

    const disabled = computed(() => docs.value.some((doc) => !doc.Optional && !doc.Files.filter((file) => !file.Deleted).length));
    const editable = computed(() => isProjectEditable("multiple", project.value.Status, 2));
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

    const save = (submit) => {
        const data = {
            ID: props.id,
            Attachments: docs.value.reduce((list, item) => list.concat(item.Files), []),
            Submit: submit ? "true" : "false"
        };

        api.multiple("saveAttachment", data).subscribe(() => {
            if (submit) {
                // TODO
                // window.location.href = `Attachment.aspx?ID=${props.id}`;
            } else {
                window.location.reload();
            }
        });
    }

    onMounted(() => {
        rxjs.forkJoin([
            api.multiple("getAttachment", { ID: props.id })
        ]).subscribe((result) => {
            const data = result[0];

            project.value = data.Project;
            docs.value = docs.value.filter((doc) => !doc.Excludes?.includes(project.value.OrgCategory));

            // TODO
            // docs.value.push({ Type: 6, Title: "著作權授權同意書", Template: "", Files: [] });

            docs.value.forEach((doc) => {
                doc.Files = data.Attachments.filter((file) => file.Type === doc.Type);
                doc.Uploaded = computed(() => doc.Files.filter((file) => !file.Deleted).length);
            });

            useProgressStore().multiple = { step: project.value.FormStep, status: project.value.Status };
        });
    });

    provide("editable", editable);
</script>
