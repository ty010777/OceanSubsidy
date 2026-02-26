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
                            <input-text disabled v-model.trim="form.ProjectName"></input-text>
                        </td>
                    </tr>
                    <tr>
                        <th><required-label>計畫類別</required-label></th>
                        <td>
                            <input-select :empty-value="null" :error="errors.Field" :options="fields" text-name="Descname" value-name="Code" v-model="form.Field"></input-select>
                        </td>
                    </tr>
                    <tr>
                        <th><required-label>申請單位</required-label></th>
                        <td>
                            <input-text :error="errors.OrgName" :max-length="100" placeholder="請輸入學校全銜" v-model.trim="form.OrgName"></input-text>
                        </td>
                    </tr>
                    <tr>
                        <th><required-label>校長姓名</required-label></th>
                        <td>
                            <input-text :error="errors.OrgLeader" :max-length="100" v-model.trim="form.OrgLeader"></input-text>
                        </td>
                    </tr>
                    <tr>
                        <th><required-label>學校地址</required-label></th>
                        <td>
                            <input-text :error="errors.Address" :max-length="100" placeholder="請輸入地址" v-model.trim="form.Address"></input-text>
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
                        <th>最近三年參與本會辦理之相關研習或課程</th>
                        <td>
                            <input-boolean v-model="hasStudies"></input-boolean>
                            <div class="table-responsive mt-3 mb-0" v-if="hasStudies">
                                <table class="table align-middle gray-table">
                                    <thead>
                                        <tr>
                                            <th>研習/課程名稱</th>
                                            <th>辦理日期</th>
                                            <th v-if="editable">功能</th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        <tr :key="item" v-for="(item, idx) in filteredStudies">
                                            <td>
                                                <input-text :error="errors[`study-${idx}-Title`]" :max-length="100" v-model.trim="item.Title"></input-text>
                                            </td>
                                            <td>
                                                <input-tw-date :error="errors[`study-${idx}-TheDate`]" v-model.trim="item.TheDate"></input-tw-date>
                                            </td>
                                            <td v-if="editable">
                                                <div class="d-flex gap-2">
                                                    <button class="btn btn-sm btn-teal-dark m-0" @click="remove(item)" type="button" v-if="filteredStudies.length > 1"><i class="fas fa-trash-alt"></i></button>
                                                    <button class="btn btn-sm btn-teal-dark m-0" @click="add" type="button" v-if="idx + 1 === filteredStudies.length"><i class="fas fa-plus"></i></button>
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
        <template v-if="changeForm">
            <h5 class="square-title mt-5">變更說明</h5>
            <div class="mt-4">
                <table class="table align-middle gray-table side-table">
                    <tbody>
                        <tr>
                            <th><required-label>變更前</required-label></th>
                            <td>
                                <input-textarea :error="errors.Form1Before" rows="4" v-model.trim="changeForm.Form1Before"></input-textarea>
                            </td>
                        </tr>
                        <tr>
                            <th><required-label>變更後</required-label></th>
                            <td>
                                <input-textarea :error="errors.Form1After" rows="4" v-model.trim="changeForm.Form1After"></input-textarea>
                            </td>
                        </tr>
                    </tbody>
                </table>
            </div>
        </template>
    </div>
    <div class="block-bottom bg-light-teal" v-if="editable">
        <button class="btn btn-outline-teal me-3" @click="save()" type="button">暫存</button>
        <button class="btn btn-teal" @click="save(true)" type="button"><i class="fas fa-check"></i>完成本頁，下一步</button>
    </div>
</template>

<script setup>
    const props = defineProps({
        apply: { default: false, type: Boolean },
        id: { type: [Number, String] }
    });

    const contacts = ref();
    const changeForm = ref();
    const editable = computed(() => isProjectEditable("literacy", form.value.Status, 1));
    const errors = ref({});
    const fields = ref([]);
    const filteredStudies = computed(() => studies.value.filter((item) => !item.Deleted));
    const form = ref({});
    const studies = ref([]);

    const hasStudies = computed({
        get: () => !!filteredStudies.value.length,
        set: (value) => {
            if (value) {
                add();
            } else {
                studies.value.forEach((item) => item.Deleted = true);
            }
        }
    });

    const add = () => studies.value.push({});

    const emit = defineEmits(["next"]);

    const load = () => {
        api.literacy("getApplication", { Apply: props.apply ? "true" : "false", ID: props.id }).subscribe((res) => {
            form.value = res.Project;
            contacts.value = res.Contacts;
            studies.value = res.PreviousStudies;

            useProgressStore().init("literacy", form.value);

            changeForm.value = form.value.changeApply;
            form.value.changeApply = undefined;
        });
    };

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
            PreviousStudies: studies.value.filter((item) => item.ID || !item.Deleted),
            Before: changeForm.value?.Form1Before,
            After: changeForm.value?.Form1After,
            Submit: submit ? "true" : "false"
        };

        api.literacy("saveApplication", data).subscribe((res) => {
            if (submit) {
                emit("next", res.ID);
            } else if (form.value.ID) {
                load();
            } else {
                window.location.href = `Application.aspx?ID=${res.ID}`;
            }
        });
    };

    const verify = () => {
        let rules = {
            Field: "計畫類別",
            OrgName: "申請單位",
            OrgLeader: "校長姓名",
            Address: "學校地址",
            Target: "計畫目標",
            Summary: "計畫內容概要",
            Quantified: "預期效益(量化)",
            Qualitative: "預期效益(質化)"
        };

        errors.value = validateData(form.value, rules);

        rules = {
            Name: "姓名",
            JobTitle: "職稱",
            MobilePhone: "手機號碼",
            EMail: "電子郵件"
        };

        contacts.value.forEach((contact, index) => Object.assign(errors.value, validateData(contact, rules, `contact-${index}-`)));

        rules = {
            Title: "研習/課程名稱",
            TheDate: "辦理日期"
        };

        studies.value.forEach((study, index) => Object.assign(errors.value, validateData(study, rules, `study-${index}-`)));

        if (changeForm.value) {
            rules = {
                Form1Before: "變更前說明",
                Form1After: "變更後說明"
            };

            Object.assign(errors.value, validateData(changeForm.value, rules));
        }

        return !Object.keys(errors.value).length;
    };

    onMounted(() => {
        rxjs.forkJoin([
            api.literacy("getZgsCodes", { CodeGroup: "LITField" })
        ]).subscribe((result) => {
            fields.value = result[0];

            if (props.id) {
                load();
            } else {
                api.literacy("getEmptyApplication").subscribe((res) => {
                    form.value = Object.assign({ Field: null, FormStep: 1, Status: 1 }, res);
                    contacts.value = [{ Role: "申請人" }];

                    useProgressStore().init("literacy", form.value);
                });
            }
        });
    });

    provide("editable", editable);

    watch(() => [form.value.Field, form.value.OrgName], () => {
        if (editable.value) {
            const names = [];

            if (form.value.Field) {
                names.push(fields.value.find((field) => field.Code === form.value.Field)?.Descname);
            }

            if (form.value.OrgName) {
                names.push(form.value.OrgName);
            }

            form.value.ProjectName = names.join("-");
        }
    });
</script>
