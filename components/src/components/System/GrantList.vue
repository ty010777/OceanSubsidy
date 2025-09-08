<template>
    <div class="search-form">
        <div class="row g-3">
            <div class="col-3">
                <div class="fs-16 text-gray mb-2">年度</div>
                <select class="form-select" v-model="year">
                    <option :key="item" :value="item" v-for="(item) in years">{{ item }}年</option>
                </select>
            </div>
        </div>
    </div>
    <div class="block rounded-4 mt-4">
        <div class="title border-teal">
            <div class="d-flex align-items-center gap-2">
                <h4 class="text-teal">
                    <img :src="`../../assets/img/title-icon02-teal.svg`" alt="logo">
                    <span>列表</span>
                </h4>
            </div>
            <a class="btn btn-teal-dark" href="GrantEdit.aspx">
                <i class="fa-solid fa-plus"></i>
                新增類別
            </a>
        </div>
        <div class="table-responsive" style="min-height:400px">
            <table aria-label="公告列表" class="table teal-table">
                <thead>
                    <tr>
                        <th scope="col">年度</th>
                        <th scope="col" class="text-start">補助類別全稱</th>
                        <th scope="col" class="text-start">簡稱</th>
                        <th scope="col">代碼</th>
                        <th scope="col">申請期間</th>
                        <th scope="col">管理</th>
                    </tr>
                </thead>
                <tbody>
                    <tr :key="item" v-for="(item) in rows">
                        <td data-th="年度:">{{ item.Year }}</td>
                        <td data-th="補助類別全稱:" class="text-start">{{ item.FullName }}</td>
                        <td data-th="簡稱:" class="text-start">{{ item.ShortName }}</td>
                        <td data-th="代碼:" class="text-start">{{ item.TypeCode }}</td>
                        <td data-th="申請期間:" class="text-start"><tw-date default-value="未設定" :value="item.ApplyStartDate"></tw-date> ~ <tw-date default-value="未設定" :value="item.ApplyEndDate"></tw-date></td>
                        <td data-th="管理:" class="text-center">
                            <div class="d-inline-flex gap-2">
                                <a class="btn btn-sm btn-teal-dark" :href="`GrantEdit.aspx?ID=${item.TypeID}`"><i class="fa-solid fa-pen"></i></a>
                            </div>
                        </td>
                    </tr>
                </tbody>
            </table>
        </div>
    </div>
</template>

<script setup>
    const list = ref([]);
    const year = ref();
    const years = ref([]);

    const rows = computed(() => list.value.filter((item) => item.Year === year.value));

    onMounted(() => {
        api.system("getGrantTypeList").subscribe((res) => {
            list.value = res.List;
            years.value = [];

            list.value.forEach((item) => {
                if (!years.value.includes(item.Year)) {
                    years.value.push(item.Year);
                }
            });

            year.value = years.value.length ? years.value[0] : null;
        });
    });
</script>
