<template>
    <div class="block">
        <h5 class="square-title">基本資料</h5>
        <div class="mt-4">
            <table class="table align-middle gray-table side-table">
                <tbody>
                    <tr>
                        <th>年度</th>
                        <td>
                            {{ form.Year }}
                        </td>
                    </tr>
                    <tr>
                        <th>計畫編號</th>
                        <td>
                            <span v-if="form.ProjectID">{{ form.ProjectID }}</span>
                            <span class="text-light-gray" v-else>儲存後由系統自動產生</span>
                        </td>
                    </tr>
                    <tr>
                        <th>補助計畫類別</th>
                        <td>
                            {{ form.SubsidyPlanType }}
                        </td>
                    </tr>
                    <tr>
                        <th><required-label>計畫名稱</required-label></th>
                        <td>
                            <input-text :error="errors.ProjectName" v-model.trim="form.ProjectName"></input-text>
                        </td>
                    </tr>
                    <tr>
                        <th><required-label>徵件類別</required-label></th>
                        <td>
                            <div class="d-flex gap-2">
                                <input-select :empty-value="null" :options="mainFields" text-name="Descname" value-name="Code" v-model="field"></input-select>
                                <input-select :empty-value="null" :disabled="!field" :options="subFields" text-name="Descname" value-name="Code" v-model="form.Field"></input-select>
                            </div>
                            <div class="invalid mt-0" v-if="errors.Field">{{ errors.Field }}</div>
                        </td>
                    </tr>
                    <tr>
                        <th><required-label>申請單位</required-label></th>
                        <td>
                            <input-text :error="errors.OrgName" placeholder="請輸入名稱" v-model.trim="form.OrgName"></input-text>
                        </td>
                    </tr>
                    <tr>
                        <th><required-label>申請單位類別</required-label></th>
                        <td>
                            <div class="d-flex flex-column gap-2">
                                <div>
                                    <input-select :empty-value="null" :error="errors.OrgCategory" :options="categories" text-name="Descname" value-name="Code" v-model="form.OrgCategory"></input-select>
                                </div>
                                <div v-if="form.OrgCategory === '3'">
                                    <input-text :error="errors.RegisteredNum" prefix="立案登記證字號" v-model.trim="form.RegisteredNum"></input-text>
                                </div>
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <th><required-label>統一編號<br />(稅籍)</required-label></th>
                        <td>
                            <input-text :error="errors.TaxID" v-model.trim="form.TaxID"></input-text>
                        </td>
                    </tr>
                    <tr>
                        <th><required-label>立案聯絡地址</required-label></th>
                        <td>
                            <input-text :error="errors.Address" placeholder="請輸入地址" v-model.trim="form.Address"></input-text>
                        </td>
                    </tr>
                </tbody>
            </table>
        </div>
        <h5 class="square-title mt-5">人員聯絡資訊</h5>
        <div class="mt-4">
            <table class="table align-middle gray-table side-table">
                <tbody>
                    <tr :key="item" v-for="(item, idx) in contacts">
                        <th><required-label>{{ item.Role }}</required-label></th>
                        <td>
                            <div class="row g-3">
                                <div class="col-12 col-xl-2">
                                    <div class="text-gray mb-2"><required-label>姓名</required-label></div>
                                    <input-text :error="errors[`contact-${idx}-Name`]" v-model.trim="item.Name"></input-text>
                                </div>
                                <div class="col-12 col-xl-3">
                                    <div class="text-gray mb-2"><required-label>職稱</required-label></div>
                                    <input-text :error="errors[`contact-${idx}-JobTitle`]" v-model.trim="item.JobTitle"></input-text>
                                </div>
                                <div class="col-12 col-xl-4">
                                    <div class="text-gray mb-2">電話(分機)</div>
                                    <div class="row g-1">
                                        <div class="col-7">
                                            <input-text v-model.trim="item.Phone"></input-text>
                                        </div>
                                        <div class="col-5">
                                            <input-text placeholder="請輸入分機" size="4" v-model.trim="item.PhoneExt"></input-text>
                                        </div>
                                    </div>
                                </div>
                                <div class="col-12 col-xl-3">
                                    <div class="text-gray mb-2"><required-label>手機號碼</required-label></div>
                                    <input-text :error="errors[`contact-${idx}-MobilePhone`]" v-model.trim="item.MobilePhone"></input-text>
                                </div>
                                <div class="col-12">
                                    <div class="text-gray mb-2"><required-label>電子郵件</required-label></div>
                                    <input-text :error="errors[`contact-${idx}-EMail`]" v-model.trim="item.EMail"></input-text>
                                </div>
                            </div>
                        </td>
                    </tr>
                </tbody>
            </table>
        </div>
        <h5 class="square-title mt-5">計畫內容</h5>
        <div class="mt-4">
            <table class="table align-middle gray-table side-table">
                <tbody>
                    <tr>
                        <th><required-label>計畫目標<br />(條列式)</required-label></th>
                        <td>
                            <div class="fs-14 mb-2">對海洋文化發展、推廣或傳承之期待，包含落實海洋基本法、2020國家海洋政策白皮書與臺灣永續發展目標</div>
                            <input-textarea :error="errors.Target" :max-length="500" placeholder="請輸入計畫目標" rows="4" v-model.trim="form.Target"></input-textarea>
                        </td>
                    </tr>
                    <tr>
                        <th><required-label>計畫內容概要<br />(條列式)</required-label></th>
                        <td>
                            <input-textarea :error="errors.Summary" :max-length="500" placeholder="請輸入計畫內容概要" rows="4" v-model.trim="form.Summary"></input-textarea>
                        </td>
                    </tr>
                    <tr>
                        <th><required-label>預期效益<br />(量化)(條列式)</required-label></th>
                        <td>
                            <input-textarea :error="errors.Quantified" :max-length="250" placeholder="請輸入預期效益" rows="4" v-model.trim="form.Quantified"></input-textarea>
                        </td>
                    </tr>
                    <tr>
                        <th><required-label>預期效益<br />(質化)(條列式)</required-label></th>
                        <td>
                            <input-textarea :error="errors.Qualitative" :max-length="250" placeholder="請輸入預期效益" rows="4" v-model.trim="form.Qualitative"></input-textarea>
                        </td>
                    </tr>
                </tbody>
            </table>
        </div>
        <div class="mt-4">
            <table class="table align-middle gray-table side-table">
                <tbody>
                    <tr>
                        <th>最近三年曾獲本會及所屬補助／合作計畫及經費</th>
                        <td>
                            <input-boolean v-model="hasReceiveds"></input-boolean>
                            <div class="table-responsive mt-3 mb-0" v-if="hasReceiveds">
                                <table class="table align-middle gray-table">
                                    <thead>
                                        <tr>
                                            <th>計畫名稱</th>
                                            <th>補助單位</th>
                                            <th class="text-end">補助金額</th>
                                            <th v-if="editable">功能</th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        <tr :key="item" v-for="(item, idx) in filteredReceiveds">
                                            <td>
                                                <input-text :error="errors[`received-${idx}-Name`]" v-model.trim="item.Name"></input-text>
                                            </td>
                                            <td>
                                                <input-text :error="errors[`received-${idx}-Unit`]" v-model.trim="item.Unit"></input-text>
                                            </td>
                                            <td>
                                                <input-integer :error="errors[`received-${idx}-Amount`]" v-model="item.Amount"></input-integer>
                                            </td>
                                            <td v-if="editable">
                                                <div class="d-flex gap-2">
                                                    <button class="btn btn-sm btn-teal-dark m-0" @click="remove(item)" type="button" v-if="filteredReceiveds.length > 1"><i class="fas fa-trash-alt"></i></button>
                                                    <button class="btn btn-sm btn-teal-dark m-0" @click="add" type="button" v-if="idx + 1 === filteredReceiveds.length"><i class="fas fa-plus"></i></button>
                                                </div>
                                            </td>
                                        </tr>
                                    </tbody>
                                </table>
                            </div>
                        </td>
                    </tr>
                </tbody>
            </table>
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

    const categories = ref([]);
    const contacts = ref();
    const editable = computed(() => isProjectEditable("culture", form.value.Status, 1));
    const errors = ref({});
    const field = ref(null);
    const fields = ref([]);
    const filteredReceiveds = computed(() => receiveds.value.filter((item) => !item.Deleted));
    const form = ref({});
    const mainFields = computed(() => fields.value.filter((item) => !item.ParentCode));
    const receiveds = ref([]);
    const subFields = computed(() => fields.value.filter((item) => item.ParentCode === field.value));

    const hasReceiveds = computed({
        get: () => !!filteredReceiveds.value.length,
        set: (value) => {
            if (value) {
                add();
            } else {
                receiveds.value.forEach((item) => item.Deleted = true);
            }
        }
    });

    const add = () => receiveds.value.push({});

    const remove = (target) => target.Deleted = true;

    const save = (submit) => {
        errors.value = {};

        if (submit && !verify()) {
            nextTick(() => document.querySelector(".invalid")?.scrollIntoView({ behavior: "smooth", block: "center" }));
            return;
        }

        const data = {
            Project: form.value,
            Contacts: contacts.value,
            ReceivedSubsidies: receiveds.value.filter((item) => item.ID || !item.Deleted),
            Submit: submit ? "true" : "false"
        };

        api.culture("saveApplication", data).subscribe((res) => {
            if (submit) {
                window.location.href = `WorkSchedule.aspx?ID=${res.ID}`;
            } else if (form.value.ID) {
                window.location.reload();
            } else {
                window.location.href = `Application.aspx?ID=${res.ID}`;
            }
        });
    };

    const verify = () => {
        let rules = {
            ProjectName: "計畫名稱",
            Field: "徵件類別",
            OrgName: "申請單位",
            OrgCategory: "申請單位類別",
            TaxID: "統一編號",
            Address: "立案聯絡地址",
            Target: "計畫目標",
            Summary: "計畫內容概要",
            Quantified: "預期效益(量化)",
            Qualitative: "預期效益(質化)"
        };

        if (form.value.OrgCategory === "3") {
            rules.RegisteredNum = "立案登記證字號";
        }

        errors.value = validateData(form.value, rules);

        rules = {
            Name: "姓名",
            JobTitle: "職稱",
            MobilePhone: "手機號碼",
            EMail: "電子郵件"
        };

        contacts.value.forEach((contact, index) => Object.assign(errors.value, validateData(contact, rules, `contact-${index}-`)));

        rules = {
            Name: "計畫名稱",
            Unit: "補助單位",
            Amount: { presence: { allowEmpty: false, message: `^請輸入補助金額` }, numericality: { greaterThan: 0, message: "^補助金額不可為 0" } }
        };

        receiveds.value.forEach((received, index) => Object.assign(errors.value, validateData(received, rules, `received-${index}-`)));

        return !Object.keys(errors.value).length;
    };

    onMounted(() => {
        rxjs.forkJoin([
            api.culture("getZgsCodes", { CodeGroup: "CULField" }),
            api.culture("getZgsCodes", { CodeGroup: "CULOrgCategory" })
        ]).subscribe((result) => {
            fields.value = result[0];
            categories.value = result[1];

            if (props.id) {
                api.culture("getApplication", { ID: props.id }).subscribe((res) => {
                    form.value = res.Project;
                    contacts.value = res.Contacts;
                    receiveds.value = res.ReceivedSubsidies;

                    field.value = fields.value.find((item) => item.Code === form.value.Field)?.ParentCode || null;
                    form.value.OrgCategory = form.value.OrgCategory || null;

                    useProgressStore().culture = { step: form.value.FormStep, status: form.value.Status };
                });
            } else {
                api.culture("getEmptyApplication").subscribe((res) => {
                    field.value = null;
                    form.value = Object.assign({ Field: null, OrgCategory: null, FormStep: 1, Status: 1 }, res);
                    contacts.value = [{ Role: "負責人" }, { Role: "聯絡人" }];

                    useProgressStore().culture = { step: form.value.FormStep, status: form.value.Status };
                });
            }
        });
    });

    provide("editable", editable);

    watch(field, () => {
        if (field.value) {
            if (!subFields.value.find((item) => form.value.Field === item.Code)) {
                form.value.Field = subFields.value[0].Code;
            }
        } else {
            form.value.Field = null;
        }
    });
</script>
