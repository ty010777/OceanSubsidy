<template>
    <div class="search bg-gray rounded-4 mt-4" v-if="isOrganizer">
        <h3 class="text-teal d-flex align-items-center gap-2">
            <img :src="`../assets/img/title-icon12-teal.svg`">
            經費執行狀況
        </h3>
        <div class="search-form">
            <div class="row g-3">
                <div class="col-12 col-lg-6">
                    <div class="fs-16 text-gray mb-2">年度</div>
                    <select class="form-select">
                        <option value="">114</option>
                    </select>
                </div>
                <div class="col-12 col-lg-6">
                    <div class="fs-16 text-gray mb-2">全部類別</div>
                    <input-select allow-empty :options="types" placeholder="全部" text-name="title" value-name="code" v-model="type"></input-select>
                </div>
            </div>
            <button class="btn btn-teal-dark d-table mx-auto" type="button">
                <i class="fa-solid fa-magnifying-glass"></i>
                查詢
            </button>
        </div>
        <div class="rounded-4 bg-white mt-4 p-4" v-if="stat">
            <div class="row">
                <div class="col-12">
                    <div class="row">
                        <div class="col-12 d-flex justify-content-between align-items-center pe-4">
                            <div class="fw-bold fs-4">總預算經費</div>
                            <div class="fw-bold text-primary fs-2">{{ Math.floor(stat.BudgetFees / 1000).toLocaleString() }} <span class="fs-6 text-black">千元</span></div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
        <div class="rounded-4 bg-white mt-4 py-4" v-if="stat">
            <div class="row">
                <div class="col-12 col-lg-4">
                    <div class="text-center fs-4 fw-bold mb-4">經費執行率</div>
                    <report-pie-chart active="已執行" :color="0x8E44AD" inactive="未執行" :percent="toPercent(stat.SpendAmount, stat.ApprovedAmount, 1)" style="width:100%;height:300px"></report-pie-chart>
                </div>
                <div class="col-12 col-lg-8">
                    <div class="d-flex justify-content-between align-items-center mb-4">
                        <span class="fs-4 fw-bold">各類經費執行情形</span>
                    </div>
                    <div class="fs-6 text-end text-secondary me-5">單位：千元</div>
                    <report-mix-chart :list="statList" style="width:100%;height:300px" v-if="statList.length"></report-mix-chart>
                </div>
            </div>
        </div>
        <div class="row" v-if="stat">
            <div class="col-12 col-lg-6 pt-4">
                <div class="d-flex justify-content-between align-items-center rounded-4 bg-white p-4">
                    <div class="fw-bold fs-4">核定補助經費</div>
                    <div class="fw-bold text-primary fs-2">{{ Math.floor(stat.ApprovedAmount / 1000).toLocaleString() }} <span class="fs-6 text-black">千元</span></div>
                </div>
            </div>
            <div class="col-12 col-lg-6 pt-4">
                <div class="d-flex justify-content-between align-items-center rounded-4 bg-white p-4">
                    <div class="fw-bold fs-4">賸餘款</div>
                    <div class="fw-bold text-primary fs-2">{{ Math.floor(stat.RemainingAmount / 1000).toLocaleString() }} <span class="fs-6 text-black">千元</span></div>
                </div>
            </div>
            <div class="col-12 col-lg-6 pt-4">
                <div class="d-flex justify-content-between align-items-center rounded-4 bg-white p-4">
                    <div class="fw-bold fs-4">實支金額</div>
                    <div class="fw-bold text-primary fs-2">{{ Math.floor(stat.SpendAmount / 1000).toLocaleString() }} <span class="fs-6 text-black">千元</span></div>
                </div>
            </div>
            <div class="col-12 col-lg-6 pt-4">
                <div class="d-flex justify-content-between align-items-center rounded-4 bg-white p-4">
                    <div class="fw-bold fs-4">已撥付</div>
                    <div class="fw-bold text-primary fs-2">{{ Math.floor(stat.PaymentAmount / 1000).toLocaleString() }} <span class="fs-6 text-black">千元</span></div>
                </div>
            </div>
        </div>
        <div class="rounded-4 bg-white p-4 mt-4 d-flex flex-column gap-3">
            <h5 class="square-title">各類經費列表</h5>
            <div class="table-responsive">
                <table class="table align-middle gray-table">
                    <thead>
                        <tr>
                            <th class="text-center">類別</th>
                            <th class="text-center">核定件數</th>
                            <th class="text-end">核定補助經費</th>
                            <th class="text-end">經費執行率</th>
                        </tr>
                    </thead>
                    <tbody>
                        <tr :key="item" v-for="(item) in statList">
                            <td class="text-center">{{ types.find((ty) => ty.code === item.Category).title }}</td>
                            <td class="text-center"><a :href="`Report/ApprovedList.aspx?category=${item.Category}`">{{ item.Count }}</a></td>
                            <td class="text-end">{{ item.ApprovedAmount.toLocaleString() }}</td>
                            <td class="text-end">{{ toPercent(item.SpendAmount, item.ApprovedAmount) }}%</td>
                        </tr>
                    </tbody>
                </table>
            </div>
        </div>
    </div>
    <div class="search bg-white rounded-4 mt-4">
        <h3 class="text-teal d-flex align-items-center gap-2">
            <img :src="`../assets/img/title-icon13-teal.svg`">
            海洋委員會補助公告
        </h3>
        <div class="table-responsive mt-4">
            <div class="row">
                <div class="col">
                    <div class="fs-16 text-gray mb-2">依身分篩選適用規定</div>
                    <select class="form-select" v-model="rank">
                        <option value="">全部</option>
                        <option :key="item" :value="item" v-for="(item) in ranks">{{ item }}</option>
                    </select>
                </div>
            </div>
            <table class="table teal-table" aria-label="申請計畫列表">
                <thead>
                    <tr>
                        <th scope="col">年度</th>
                        <th scope="col" class="text-start">類別</th>
                        <th scope="col">補助計畫</th>
                        <th scope="col">申請期限</th>
                        <th scope="col">補助對象</th>
                        <th scope="col">相關檔案</th>
                        <th scope="col"></th>
                    </tr>
                </thead>
                <tbody>
                    <tr :key="item" v-for="(item) in filterGrants">
                        <td data-th="年度:">{{ item.Year }}</td>
                        <td data-th="類別:" class="text-start">{{ item.ShortName }}</td>
                        <td data-th="補助計畫:">{{ item.FullName }}</td>
                        <td data-th="申請期限:"><tw-date :value="item.ApplyEndDate"></tw-date></td>
                        <td data-th="補助對象:">
                            <tooltip data-bs-html="true" :title="notes(item)"></tooltip>
                        </td>
                        <td data-th="相關檔案:">
                            <a class="btn btn-sm btn-teal-dark" :download="item.Filename" :href="download(item.Path)" style="display:inline" v-if="item.Path">
                                <i class="fas fa-file-alt"></i>
                            </a>
                        </td>
                        <td data-th="我要申請:">
                            <a class="btn btn-teal-dark" :href="path(item)" target="_blank"><i class="fa-solid"></i>我要申請</a>
                        </td>
                    </tr>
                </tbody>
            </table>
        </div>
    </div>
    <div class="search bg-white rounded-4 mt-4">
        <h3 class="text-teal d-flex align-items-center gap-2">
            <img :src="`../assets/img/title-icon14-teal.svg`">
            我的計畫進度
        </h3>
        <div class="mt-4">
            <ul class="total-list mt-4" v-if="isUser">
                <li class="total-item" :key="item" v-for="(item) in countList">
                    <a :href="item.url">
                        <div class="total-item-title">{{ item.title }}</div>
                        <div class="total-item-content">
                            <span class="count">{{ item.count }}</span>
                            <span class="unit">件</span>
                        </div>
                    </a>
                </li>
            </ul>
        </div>
    </div>
</template>

<script setup>
    const types = useGrantStore().typeList;

    const countList = [
        { title: "申請中尚未提送", url: "ApplicationChecklist.aspx", count: computed(() => applies.value.filter((i) => [1,14].includes(i.Status)).length) },
        { title: "申請審查中", url: "ApplicationChecklist.aspx", count: computed(() => applies.value.filter((i) => [11,12,21,22,31,32,41,42,43,44].includes(i.Status)).length) },
        { title: "審查意見待回覆", url: "ApplicationChecklist.aspx?waitingReply=1", count: computed(() => pending.value.find((i) => i.Status === 1)?.Count || 0) },
        { title: "執行中計畫", url: "inprogressList.aspx", count: computed(() => applies.value.filter((i) => [51,52].includes(i.Status)).length) },
        { title: "執行階段待回覆", url: "inprogressList.aspx?pendingReply=1", count: computed(() => pending.value.find((i) => i.Status === 2)?.Count || 0) }
    ];

    const applies = ref([]);
    const grants = ref([]);
    const isOrganizer = ref();
    const isUser = ref();
    const pending = ref([]);
    const rank = ref("");
    const ranks = ref([]);
    const settings = ref([]);
    const statList = ref([]);
    const type = ref();

    const filterGrants = computed(() => {
        let data = grants.value;

        if (rank.value) {
            data = data.filter((item) => item.tags.includes(rank.value));
        }

        return data;
    });

    const filterStatList = computed(() => {
        if (type.value) {
            return statList.value.filter((item) => item.Category === type.value);
        }

        return statList.value;
    });

    const stat = computed(() => {
        return {
            ApprovedAmount: filterStatList.value.reduce((sum, item) => sum + item.ApprovedAmount, 0),
            RemainingAmount: filterStatList.value.reduce((sum, item) => sum + item.ApprovedAmount - item.SpendAmount, 0),
            SpendAmount: filterStatList.value.reduce((sum, item) => sum + item.SpendAmount, 0),
            PaymentAmount: filterStatList.value.reduce((sum, item) => sum + item.PaymentAmount, 0),
            BudgetFees: filterStatList.value.reduce((sum, item) => sum + item.BudgetFees, 0)
        };
    });

    const download = api.download;

    const notes = (item) => settings.value.filter((i) => i.GrantTypeID === item.TypeCode).map((i, idx) => `${idx+1}.${i.Note}`).join("<br>");

    const path = (item) => {
        switch (item.TypeCode) {
            case "SCI":
                return `SCI/SciApplication.aspx?TypeID=${item.TypeID}`;
            case "CLB":
                return `CLB/ClbApplication.aspx?TypeID=${item.TypeID}`;
            default:
                return `${item.TypeCode}/Application.aspx`;
        }
    };

    const toPercent = (value1, value2, digits = 2) => {
        if (value2) {
            const base = 10 ** digits;

            return Math.round(value1 * 100 / value2 * base) / base;
        }

        return 0;
    };

    onMounted(() => {
        api.system("dashboard").subscribe((res) => {
            grants.value = res.GrantTypes;
            settings.value = res.Settings;
            applies.value = res.ApplyList;
            statList.value = res.StatList;
            pending.value = res.PendingList;
            isOrganizer.value = res.IsOrganizer;
            isUser.value = res.IsUser;

            grants.value.forEach((item) => {
                item.tags = item.TargetTags?.split("\n").map((tag) => tag.trim()).filter((tag) => tag) || [];

                item.tags.forEach((tag) => {
                    if (!ranks.value.includes(tag)) {
                        ranks.value.push(tag);
                    }
                });
            });
        });
    });
</script>
