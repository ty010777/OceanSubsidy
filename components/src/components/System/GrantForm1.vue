<template>
    <div class="block">
        <table class="table align-middle gray-table side-table">
            <tbody>
                <tr>
                    <th><required-label>補助類別(代碼)</required-label></th>
                    <td>
                        <input-select class="w-auto" :error="errors.TypeCode" :options="types" v-model="form.TypeCode">
                            <template #default="{ option }">
                                <option :value="option.code">{{ option.title }} ({{ option.code }})</option>
                            </template>
                        </input-select>
                    </td>
                </tr>
                <tr>
                    <th><required-label>年度</required-label></th>
                    <td>
                        <input-integer class="w-auto" :error="errors.Year" v-model.trim="form.Year"></input-integer>
                    </td>
                </tr>
                <tr>
                    <th><required-label>補助類別全稱</required-label></th>
                    <td>
                        <input-text :error="errors.FullName" v-model.trim="form.FullName"></input-text>
                    </td>
                </tr>
                <tr>
                    <th><required-label>內容</required-label></th>
                    <td>
                        <input-textarea :error="errors.ServiceContent" v-model.trim="content.ServiceContent"></input-textarea>
                    </td>
                </tr>
                <tr>
                    <th>申辦資格</th>
                    <td>
                        <input-textarea v-model.trim="content.Criteria"></input-textarea>
                    </td>
                </tr>
                <tr>
                    <th><required-label>申辦流程</required-label></th>
                    <td>
                        <table class="table align-middle gray-table">
                            <tbody>
                                <tr :key="item" v-for="(item,idx) in filterProcedures">
                                    <td width="1%">{{ idx + 1 }}</td>
                                    <td><input-textarea :error="errors[`procedure-${idx}-Content`]" v-model.trim="item.Content"></input-textarea></td>
                                    <td width="1%">
                                        <div class="d-flex gap-2">
                                            <button class="btn btn-sm btn-teal-dark m-0" @click="item.Deleted = true" type="button" v-if="filterProcedures.length > 1">
                                                <i class="fas fa-trash-alt"></i>
                                            </button>
                                            <button class="btn btn-sm btn-teal-dark m-0" @click="procedures.push({})" type="button" v-if="idx + 1 === filterProcedures.length">
                                                <i class="fas fa-plus"></i>
                                            </button>
                                        </div>
                                    </td>
                                </tr>
                            </tbody>
                        </table>
                    </td>
                </tr>
                <tr>
                    <th>應備物品</th>
                    <td>
                        <input-textarea v-model.trim="content.Documentary"></input-textarea>
                    </td>
                </tr>
                <tr>
                    <th><required-label>聯絡窗口</required-label></th>
                    <td>
                        <div class="d-flex align-items-center flex-wrap">
                            <div class="d-flex align-items-center text-nowrap me-3">
                                <label class="me-2">聯絡電話及分機</label>
                                <input-text v-model.trim="content.ContactTel"></input-text>
                            </div>
                            <div class="d-flex align-items-center text-nowrap">
                                <label class="me-2">聯絡人</label>
                                <input-text v-model.trim="content.ContactPerson"></input-text>
                            </div>
                        </div>
                        <div class="invalid mt-0" v-if="errors.ContactTel || errors.ContactPerson">{{ errors.ContactTel || errors.ContactPerson }}</div>
                    </td>
                </tr>
                <tr>
                    <th>線上申辦<br>參考資料</th>
                    <td>
                        <table class="table align-middle gray-table" v-if="filterLinks.length">
                            <tbody>
                                <tr :key="item" v-for="(item,idx) in filterLinks">
                                    <td width="1%">{{ idx + 1 }}</td>
                                    <td><input-text :error="errors[`link-${idx}-URL`]" v-model.trim="item.URL"></input-text></td>
                                    <td width="1%">
                                        <div class="d-flex gap-2">
                                            <button class="btn btn-sm btn-teal-dark m-0" @click="item.Deleted = true" type="button">
                                                <i class="fas fa-trash-alt"></i>
                                            </button>
                                        </div>
                                    </td>
                                </tr>
                            </tbody>
                        </table>
                        <button class="btn btn-teal-dark" @click="links.push({})" type="button"><i class="fas fa-plus"></i>新增</button>
                    </td>
                </tr>
                <tr>
                    <th><required-label>相關檔案<br>(顯示於首頁)</required-label></th>
                    <td>
                        <div class="tag-group mt-2 gap-1" v-if="file">
                            <span class="tag tag-green-light">
                                <a class="tag-link my-1" :download="file.Name" :href="download(file.Path)">{{ file.Name }}</a>
                                <button class="tag-btn" @click="file = null" type="button">
                                    <i class="fa-solid fa-circle-xmark"></i>
                                </button>
                            </span>
                        </div>
                        <div v-else>
                            <input-file @file="(newFile) => file = newFile"></input-file>
                        </div>
                        <div class="invalid mt-0" v-if="errors.Filename">{{ errors.Filename }}</div>
                    </td>
                </tr>
                <tr v-if="content.IsValid">
                    <th>申請下架</th>
                    <td>
                        <input class="form-check-input" id="status-checkbox" type="checkbox" v-model="checked" />
                        <label class="form-check-label ms-1 mb-2" for="status-checkbox">申請E政府下架（請輸入原因）</label>
                        <input-textarea :error="errors.StatusReason" v-model.trim="content.StatusReason"></input-textarea>
                    </td>
                </tr>
            </tbody>
        </table>
        <div class="d-flex gap-3 flex-wrap justify-content-center mt-4">
            <button class="btn btn-outline-teal me-3" @click="save()" type="button">暫存</button>
            <button class="btn btn-teal" @click="save(true)" type="button"><i class="fas fa-check"></i>確定發布</button>
        </div>
    </div>
</template>

<script setup>
    const props = defineProps({
        id: { type: [Number, String] }
    });

    const types = useGrantStore().typeList;

    const checked = ref(false);
    const content = ref({});
    const errors = ref({});
    const file = ref();
    const filterProcedures = computed(() => procedures.value.filter((item) => !item.Deleted));
    const filterLinks = computed(() => links.value.filter((item) => !item.Deleted));
    const form = ref({});
    const links = ref([]);
    const procedures = ref([]);

    const download = api.download;

    const emit = defineEmits(["next"]);

    const save = (submit) => {
        errors.value = {};

        content.value.Path = file.value?.Path;
        content.value.Filename = file.value?.Name;

        if (submit && !verify()) {
            nextTick(() => document.querySelector(".invalid")?.scrollIntoView({ behavior: "smooth", block: "center" }));
            return;
        }

        form.value.ShortName = types.find((type) => type.code === form.value.TypeCode)?.title;

        const data = {
            GrantType: form.value,
            Content: content.value,
            Procedures: procedures.value,
            Links: links.value
        };

        api.system("saveGrantTypeContent", data).subscribe((res) => {
            if (res) {
                emit("next");
            }
        });
    };

    const verify = () => {
        let rules = {
            ServiceContent: "內容",
            ContactPerson: "聯絡電話及分機",
            ContactTel: "聯絡人",
            Filename: "相關檔案"
        };

        if (checked.value) {
            rules.StatusReason = "下架原因";
        }

        errors.value = validateData(content.value, rules);

        if (!props.id) {
            rules = {
                TypeCode: "補助類別",
                Year: "年度",
                FullName: "補助類別全稱"
            };

            Object.assign(errors.value, validateData(form.value, rules));
        }

        if (filterProcedures.value.length) {
            rules = {
                Content: "內容"
            };

            filterProcedures.value.forEach((procedure, idx) => Object.assign(errors.value, validateData(procedure, rules, `procedure-${idx}-`)));
        }

        if (filterLinks.value.length) {
            rules = {
                URL: "網址"
            };

            filterLinks.value.forEach((link, idx) => Object.assign(errors.value, validateData(link, rules, `link-${idx}-`)));
        }

        return !Object.keys(errors.value).length;
    };

    onMounted(() => {
        if (props.id) {
            api.system("getGrantTypeContent", { ID: props.id }).subscribe((res) => {
                form.value = res.GrantType;
                content.value = res.Content;
                procedures.value = res.Procedures;
                links.value = res.Links;

                if (content.value.Filename) {
                    file.value = { Name: content.value.Filename, Path: content.value.Path };
                }

                if (!procedures.value.length) {
                    procedures.value.push({});
                }

                useGrantStore().init(form.value.TypeCode);
            });
        } else {
            procedures.value.push({});
        }
    });
</script>
