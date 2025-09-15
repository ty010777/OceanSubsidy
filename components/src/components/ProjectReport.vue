<template>
    <div class="block">
        <h5 class="square-title">{{ data.subtitle }}</h5>
        <p class="text-pink mt-3 lh-base" v-html="data.description"></p>
        <div class="table-responsive mt-3">
            <table class="table align-middle gray-table">
                <thead>
                    <tr>
                        <th width="70" class="text-center">附件編號</th>
                        <th>附件名稱</th>
                        <th class="text-center">狀態</th>
                        <th>上傳附件</th>
                    </tr>
                </thead>
                <tbody>
                    <tr :key="item" v-for="(item, idx) in docs">
                        <td class="text-center">
                            {{ idx + 1 }}
                        </td>
                        <td>
                            <div>
                                <required-label>{{ item.Title }}</required-label>
                            </div>
                            <a class="btn btn-sm btn-teal-dark rounded-pill mt-2" :href="item.Template" style="width:140px" type="button" v-if="editable && item.Template">
                                <i class="fas fa-file-download me-1"></i>範本下載
                            </a>
                        </td>
                        <td class="text-center" v-if="editable">
                            <span v-if="item.Uploaded">已上傳</span>
                            <span class="text-pink" v-else>尚未上傳</span>
                        </td>
                        <td>
                            <input-file accept=".zip" @file="(file) => add(item, file)" v-if="editable && !item.Uploaded"></input-file>
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
        <button class="btn btn-teal" @click="save(true)" :disabled="disabled" type="button"><i class="fas fa-check"></i>提送</button>
    </div>
</template>

<script setup>
    let version;

    const props = defineProps({
        data: { required: true, type: Object },
        id: { type: [Number, String] },
        type: { type: String }
    });

    const disabled = computed(() => docs.value.some((doc) => !doc.Optional && !doc.Files.filter((file) => !file.Deleted).length));
    const docs = ref(props.data.docs.map((doc) => ({ ...doc, Files: [] })));
    const editable = ref();
    const report = ref({});

    const add = (item, file) => {
        if (file.Type === "application/zip") {
            item.Files.push({
                Type: item.Type,
                Path: file.Path,
                Name: `${props.id}_${item.Title}_${version}.zip`
            });
        }
    };

    const download = api.download;

    const emit = defineEmits(["next"]);

    const load = () => {
        api[props.type]("getReport", { ID: props.id, Stage: props.data.id }).subscribe((res) => {
            report.value = res.Report;

            docs.value.forEach((doc) => {
                doc.Files = res.Attachments.filter((file) => file.Type === doc.Type);
                doc.Uploaded = computed(() => doc.Files.filter((file) => !file.Deleted).length);
            });

            editable.value = !report.value.Status || report.value.Status === "暫存";

            version = report.value?.Reviewer ? "修訂版" : "初版";

            useProgressStore().init(props.type, res.Project, report.value);
        });
    };

    const save = (submit) => {
        const data = {
            ID: props.id,
            Stage: props.data.id,
            Attachments: docs.value.reduce((list, item) => list.concat(item.Files), []),
            Submit: submit ? "true" : "false"
        };

        api[props.type]("saveReport", data).subscribe(() => {
            if (submit) {
                emit("next");
            } else {
                load();
            }
        });
    };

    onMounted(load);

    provide("editable", editable);
</script>
