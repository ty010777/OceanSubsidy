<template>
    <div class="search bg-gray mt-4">
        <h3 class="text-teal">
            <i class="fa-solid fa-magnifying-glass"></i>
            查詢
        </h3>
        <div class="search-form">
            <div class="row g-3">
                <div class="col-12 col-lg-4">
                    <div class="fs-16 text-gray mb-2">年度</div>
                    <select class="form-select">
                        <option>114年</option>
                    </select>
                </div>
                <div class="col-12 col-lg-4">
                    <div class="fs-16 text-gray mb-2">類別</div>
                    <select class="form-select" v-model="form.category">
                        <option :value="undefined">全部</option>
                        <option :key="item" :value="item" v-for="(item) in categoryList">{{ item }}</option>
                    </select>
                </div>
                <div class="col-12 col-lg-4">
                    <div class="fs-16 text-gray mb-2">主管單位</div>
                    <select class="form-select" v-model="form.unit">
                        <option :value="undefined">全部</option>
                        <option :key="item" :value="item" v-for="(item) in unitList">{{ item }}</option>
                    </select>
                </div>
            </div>
            <div class="row g-3">
                <div class="col-12 col-lg-6">
                    <div class="fs-16 text-gray mb-2">申請補助單位</div>
                    <input-text v-model.trim="form.org"></input-text>
                </div>
                <div class="col-12 col-lg-6">
                    <div class="fs-16 text-gray mb-2">計畫編號, 計畫名稱</div>
                    <input-text v-model.trim="form.name"></input-text>
                </div>
            </div>
            <button class="btn btn-teal-dark d-table mx-auto" type="button">
                <i class="fa-solid fa-magnifying-glass"></i>
                查詢
            </button>
        </div>
    </div>
    <ul class="total-list multiple-option">
        <li class="all-total">
            <div class="total-item-title">總申請</div>
            <div class="total-item-content">
                <span class="count">{{ filterList.length }}</span>
                <span class="unit">件</span>
            </div>
        </li>
        <li class="total-item" :key="item" v-for="(item) in statusList">
            <label class="checke-total" :for="`apply-${item.id}`">
                <div>
                    <div class="total-item-title">{{ item.title }}</div>
                    <div class="total-item-content">
                        <span class="count">{{ filterList.filter((i) => i.type === item.id).length }}</span>
                        <span class="unit">件</span>
                    </div>
                </div>
                <img class="check-icon" :src="`../../assets/img/check-white.svg`">
            </label>
            <input class="total-checkbox" :id="`apply-${item.id}`" type="checkbox" v-model="item.checked" />
        </li>
    </ul>
    <div class="block rounded-bottom-4">
        <div class="title border-teal">
            <div class="d-flex align-items-center gap-2">
                <h4 class="text-teal">
                    <img :src="`../../assets/img/title-icon02-teal.svg`" alt="logo">
                    <span>列表</span>
                </h4>
                <span>共 <span class="text-teal">{{ checkedList.length }}</span> 筆資料</span>
            </div>
            <button class="btn btn-teal-dark" type="button"><i class="fas fa-download"></i>匯出</button>
        </div>
        <div class="table-responsive" style="min-height:400px">
            <table class="table teal-table">
                <thead>
                    <tr>
                        <th width="80">年度</th>
                        <th>
                            <div class="hstack align-items-center justify-content-center">
                                <span>類別</span>
                            </div>
                        </th>
                        <th width="240">
                            <div class="hstack align-items-center">
                                <span>計畫名稱</span>
                            </div>
                        </th>
                        <th width="240">
                            <div class="hstack align-items-center">
                                <span>申請單位</span>
                            </div>
                        </th>
                        <th>
                            <div class="hstack align-items-center justify-content-end">
                                <span>申請本會補助</span>
                            </div>
                        </th>
                        <th>
                            <div class="hstack align-items-center justify-content-end">
                                <span>配合款</span>
                            </div>
                        </th>
                        <th>
                            <div class="hstack align-items-center justify-content-end">
                                <span>總經費</span>
                            </div>
                        </th>
                        <th>
                            <div class="hstack align-items-center justify-content-center">
                                <span>狀態</span>
                            </div>
                        </th>
                    </tr>
                    <tr class="total-row">
                        <td colspan="4" style="text-align:end">總計</td>
                        <td class="text-end">{{ checkedList.reduce((sum, item) => sum + item.ApplyAmount, 0).toLocaleString() }}</td>
                        <td class="text-end">{{ checkedList.reduce((sum, item) => sum + item.OtherAmount, 0).toLocaleString() }}</td>
                        <td class="text-end">{{ checkedList.reduce((sum, item) => sum + item.ApplyAmount + item.OtherAmount, 0).toLocaleString() }}</td>
                        <td></td>
                    </tr>
                </thead>
                <tbody>
                    <tr :key="item" v-for="(item) in rows">
                        <td data-th="年度:">{{ item.Year }}</td>
                        <td data-th="類別:">{{ item.Category }}</td>
                        <td data-th="計畫名稱:" style="text-align:left">
                            <a class="link-black" :href="path(item)" target="_blank">{{ item.ProjectName || "[未設定]" }}</a>
                        </td>
                        <td data-th="申請單位:" style="text-align:left">{{ item.OrgName }}</td>
                        <td data-th="申請本會補助:" class="text-end">{{ item.ApplyAmount.toLocaleString() }}</td>
                        <td data-th="配合款:" class="text-end">{{ item.OtherAmount.toLocaleString() }}</td>
                        <td data-th="總經費:" class="text-end">{{ (item.ApplyAmount + item.OtherAmount).toLocaleString() }}</td>
                        <td data-th="狀態:" style="text-align: center; width:100px">
                            <span class="text-teal">{{ statusList.find((i) => i.id === item.type).title }}</span>
                        </td>
                    </tr>
                </tbody>
            </table>
        </div>
        <list-pager :count="checkedList.length" @change="change" :page="page" :page-size="size"></list-pager>
    </div>
</template>

<script setup>
    const statusList = ref([
        { id: 1, title: "申請中", checked: true },
        { id: 2, title: "審查中", checked: true },
        { id: 3, title: "已核定", checked: true },
        { id: 4, title: "未通過", checked: true }
    ]);

    const categoryList = ref([]);
    const form = ref({});
    const list = ref([]);
    const page = ref(1);
    const size = ref(10);
    const unitList = ref([]);

    const checkedList = computed(() => filterList.value.filter((item) => statusList.value.find((s) => s.checked && item.type === s.id)));
    const rows = computed(() => checkedList.value.slice((page.value - 1) * size.value, page.value * size.value));

    const filterList = computed(() => {
        let data = list.value;

        if (form.value.category) {
            data = data.filter((i) => i.Category === form.value.category);
        }

        if (form.value.unit) {
            data = data.filter((i) => i.SupervisoryUnit === form.value.unit);
        }

        if (form.value.org) {
            data = data.filter((i) => i.OrgName?.includes(form.value.org));
        }

        if (form.value.name) {
            data = data.filter((i) => i.ProjectID.includes(form.value.name) || i.ProjectName?.includes(form.value.name));
        }

        return data;
    });

    const change = (to, newSize) => {
        page.value = parseInt(to);
        size.value = parseInt(newSize);
    };

    const path = (item) => {
        if (item.ProjectID.includes("CUL")) {
            return `../CUL/Application.aspx?ID=${item.ProjectID}`;
        } else if (item.ProjectID.includes("EDC")) {
            return `../EDC/Application.aspx?ID=${item.ProjectID}`;
        } else if (item.ProjectID.includes("MUL")) {
            return `../MUL/Application.aspx?ID=${item.ProjectID}`;
        } else if (item.ProjectID.includes("LIT")) {
            return `../LIT/Application.aspx?ID=${item.ProjectID}`;
        } else if (item.ProjectID.includes("ACC")) {
            return `../ACC/Application.aspx?ID=${item.ProjectID}`;
        } else if (item.ProjectID.includes("CLB")) {
            return `../CLB/ClbApplication.aspx?ProjectID=${item.ProjectID}`;
        } else if (item.ProjectID.includes("SCI")) {
            return `../SCI/SciApplication.aspx?ProjectID=${item.ProjectID}`;
        }
    };

    onMounted(() => {
        api.system("getApplyList").subscribe((res) => {
            list.value = res.List;

            list.value.forEach((item) => {
                if ([1,14].includes(item.Status)) {
                    item.type = 1;
                } else if ([11,12,21,22,31,32,41,42,43,44].includes(item.Status)) {
                    item.type = 2;
                } else if ([45,51,52,91].includes(item.Status)) {
                    item.type = 3;
                } else {
                    item.type = 4;
                }

                if (!categoryList.value.includes(item.Category)) {
                    categoryList.value.push(item.Category);
                }

                if (!unitList.value.includes(item.SupervisoryUnit)) {
                    unitList.value.push(item.SupervisoryUnit);
                }
            });
        });
    });
</script>
