<%@ Page Language="C#" MasterPageFile="~/OSI/OSIMaster.master" AutoEventWireup="true" CodeFile="Dashboard.aspx.cs" Inherits="OSI_Dashboard" %>

<asp:Content ID="cTitle" ContentPlaceHolderID="TitleContent" runat="server">
    活動儀表板  | 海洋科學調查活動填報系統
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <script src="<%= ResolveUrl("~/lib/chartjs/dist/chart.umd.js") %>"></script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="Bread" runat="server">
    <img src="<%= ResolveUrl("~/assets/img/title-icon11.svg") %>" alt="logo">
    <div>
        <span>目前位置</span>
        <h2>活動儀表板</h2>
    </div>1
</asp:Content>

<asp:Content ID="cMain" ContentPlaceHolderID="MainContent" runat="server">
    <asp:UpdatePanel ID="upSearch" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <div class="search rounded-4 mb-4">
                <h3>
                    <i class="fa-solid fa-magnifying-glass"></i>
                    查詢
                </h3>

                <div class="search-form">
                    <div class="row-line">
                        <div class="search-item">
                            <div class="fs-16 text-gray mb-2">填報週期</div>
                            <asp:DropDownList
                                ID="ddlPeriodYear"
                                runat="server"
                                CssClass="form-select"
                                Style="width: 260px;"
                                AutoPostBack="true"
                                OnSelectedIndexChanged="ddlPeriodYear_SelectedIndexChanged" />
                        </div>
                        <div class="search-item">
                            <asp:DropDownList
                                ID="ddlPeriodQuarter"
                                runat="server"
                                CssClass="form-select"
                                Style="width: 180px;" />
                        </div>
                        <asp:LinkButton
                            ID="btnQuery"
                            runat="server"
                            CssClass="btn btn-blue"
                            OnClick="btnQuery_Click">
                <i class="fa-solid fa-magnifying-glass"></i>查詢
                        </asp:LinkButton>
                    </div>
                </div>

            </div>
        </ContentTemplate>
        <Triggers>
            <asp:AsyncPostBackTrigger
                ControlID="ddlPeriodYear"
                EventName="SelectedIndexChanged" />
        </Triggers>
    </asp:UpdatePanel>


    <div class="row g-3">
        <div class="col-12 col-xl-7">
            <div class="small-block rounded-4">
                <div class="small-block-title">
                    <h5>近三年活動調查案件數</h5>
                </div>
                <canvas id="chartYear"></canvas>
            </div>
        </div>
        <div class="col-12 col-xl-5">
            <div class="small-block rounded-4">
                <div class="small-block-title">
                    <h5>比例分析</h5>
                    <asp:DropDownList
                        ID="ddlPie"
                        runat="server"
                        CssClass="form-select"
                        Style="width: 220px;"
                        AutoPostBack="false">
                    </asp:DropDownList>
                </div>
                <canvas id="chartPie" style="max-height: 360px;"></canvas>
            </div>
        </div>
        <div class="col-12 col-xl-6">
            <div class="small-block rounded-4">
                <div class="small-block-title">
                    <h5>活動分布行政區分析</h5>
                </div>
                <canvas id="chartRegion"></canvas>
            </div>
        </div>
        <div class="col-12 col-xl-6">
            <div class="small-block rounded-4">
                <div class="small-block-title">
                    <h5>單位活動填報情形分析</h5>
                </div>
                <canvas id="chartUnit"></canvas>
            </div>
        </div>
    </div>

    <script type="text/javascript">
        //── 全域變數 ──
        var yearChart = null,
            pieChart = null,
            regionChart = null,
            unitChart = null;

        //── 計算中華民國年 ──
        var now = new Date(),
            rocThis = now.getFullYear() - 1911,
            rocLast = rocThis - 1,
            rocBefore = rocThis - 2;

        function renderCharts(yearData, pieData, regionData, unitData) {
            // （1）年度案件數：固定四季、補 0、destroy→new
            var quarters = ['第一季', '第二季', '第三季', '第四季'];
            var thisVals = quarters.map(q => {
                var found = yearData.find(x => x.Quarter === q);
                return found ? found.ThisYear : 0;
            });
            var lastVals = quarters.map(q => {
                var found = yearData.find(x => x.Quarter === q);
                return found ? found.LastYear : 0;
            });
            var beforeVals = quarters.map(q => {
                var found = yearData.find(x => x.Quarter === q);
                return found ? found.BeforeLastYear : 0;
            });

            var ctxY = document.getElementById('chartYear').getContext('2d');
            if (yearChart) { yearChart.destroy(); yearChart = null; }
            yearChart = new Chart(ctxY, {
                type: 'bar',
                data: {
                    labels: quarters,
                    datasets: [
                        { label: rocThis + '年', data: thisVals },
                        { label: rocLast + '年', data: lastVals },
                        { label: rocBefore + '年', data: beforeVals }
                    ]
                },
                options: {
                    responsive: true,
                    interaction: { mode: 'index' },
                    scales: {
                        x: { stacked: false, title: { display: true, text: '季度' } },
                        y: { beginAtZero: true, title: { display: true, text: '活動件數' } }
                    }
                }
            });

            // （2）比例分析：destroy→new 或 清畫布＋提示
            var canvasP = document.getElementById('chartPie'),
                ctxP = canvasP.getContext('2d');
            if (pieChart) { pieChart.destroy(); pieChart = null; }
            if (pieData && pieData.length > 0) {
                pieChart = new Chart(ctxP, {
                    type: 'pie',
                    data: {
                        labels: pieData.map(x => x.Name),
                        datasets: [{
                            data: pieData.map(x => x.Value)
                        }]
                    },
                    options: { responsive: true, maintainAspectRatio: false, plugins: { legend: { position: 'top' } } }
                });
            } else {
                ctxP.clearRect(0, 0, canvasP.width, canvasP.height);
                ctxP.font = '16px sans-serif';
                ctxP.textAlign = 'center';
                ctxP.fillText('無資料', canvasP.width / 2, canvasP.height / 2);
            }

            // （3）行政區分佈：水平堆疊長條
            var canvasR = document.getElementById('chartRegion'),
                ctxR = canvasR.getContext('2d');
            if (regionChart) { regionChart.destroy(); regionChart = null; }
            if (regionData && regionData.length > 0) {
                // 收集所有不同的系列名稱（單位名稱）
                var allSeriesNames = [];
                regionData.forEach(function(region) {
                    if (region.Series) {
                        region.Series.forEach(function(series) {
                            if (allSeriesNames.indexOf(series.Name) === -1) {
                                allSeriesNames.push(series.Name);
                            }
                        });
                    }
                });
                
                // 為每個系列（單位）建立一個 dataset
                var datasets = allSeriesNames.map(function(seriesName) {
                    return {
                        label: seriesName,
                        data: regionData.map(function(region) {
                            // 在這個地區找到對應的系列值
                            var series = region.Series ? region.Series.find(s => s.Name === seriesName) : null;
                            return series ? series.Value : 0;
                        }),
                        stack: 'stack0'
                    };
                });
                
                regionChart = new Chart(ctxR, {
                    type: 'bar',
                    data: {
                        labels: regionData.map(r => r.RegionName),
                        datasets: datasets
                    },
                    options: {
                        indexAxis: 'y',
                        responsive: true,
                        scales: { x: { stacked: true }, y: { stacked: true } }
                    }
                });
            } else {
                ctxR.clearRect(0, 0, canvasR.width, canvasR.height);
                ctxR.font = '16px sans-serif';
                ctxR.textAlign = 'center';
                ctxR.fillText('無資料', canvasR.width / 2, canvasR.height / 2);
            }

            // （4）單位填報情形：水平群組長條
            var canvasU = document.getElementById('chartUnit'),
                ctxU = canvasU.getContext('2d');
            if (unitChart) { unitChart.destroy(); unitChart = null; }
            if (unitData && unitData.length > 0) {
                // 收集所有不同的縣市名稱
                var allCounties = [];
                unitData.forEach(function(unit) {
                    if (unit.Series) {
                        unit.Series.forEach(function(series) {
                            if (allCounties.indexOf(series.Year) === -1) {
                                allCounties.push(series.Year);
                            }
                        });
                    }
                });
                
                // 為每個縣市建立一個 dataset
                var datasets = allCounties.map(function(county) {
                    return {
                        label: county,
                        data: unitData.map(function(unit) {
                            // 在這個單位找到對應的縣市值
                            var series = unit.Series ? unit.Series.find(s => s.Year === county) : null;
                            return series ? series.Count : 0;
                        }),
                        stack: 'stack0'  // 加入堆疊設定
                    };
                });
                
                unitChart = new Chart(ctxU, {
                    type: 'bar',
                    data: {
                        labels: unitData.map(u => u.UnitName),
                        datasets: datasets
                    },
                    options: { 
                        indexAxis: 'y', 
                        responsive: true, 
                        scales: { 
                            x: { 
                                stacked: true,  // X軸堆疊
                                beginAtZero: true 
                            },
                            y: { 
                                stacked: true   // Y軸堆疊
                            }
                        } 
                    }
                });
            } else {
                ctxU.clearRect(0, 0, canvasU.width, canvasU.height);
                ctxU.font = '16px sans-serif';
                ctxU.textAlign = 'center';
                ctxU.fillText('無資料', canvasU.width / 2, canvasU.height / 2);
            }
        }

        // 等待 DOM 完全載入
        document.addEventListener('DOMContentLoaded', function () {
            //── 只更新圓餅圖：監聽 ddlPie change，用 fetch 呼叫 WebMethod ──
            var pieDdl = document.getElementById('<%= ddlPie.ClientID %>');
            pieDdl.addEventListener('change', function () {
                var period = document.getElementById('<%= ddlPeriodQuarter.ClientID %>').value;
                var category = pieDdl.value;

                fetch('Dashboard.aspx/GetPieChartData', {
                    method: 'POST',
                    headers: { 'Content-Type': 'application/json; charset=utf-8' },
                    body: JSON.stringify({ periodID: period, categoryID: category })
                })
                    .then(resp => resp.json())
                    .then(json => {
                        var data = json.d || [];
                        // destroy + new 或 清畫布
                        var canvasP = document.getElementById('chartPie'),
                            ctxP = canvasP.getContext('2d');
                        if (pieChart) { pieChart.destroy(); pieChart = null; }
                        if (data.length > 0) {
                            pieChart = new Chart(ctxP, {
                                type: 'pie',
                                data: {
                                    labels: data.map(x => x.Name),
                                    datasets: [{ data: data.map(x => x.Value) }]
                                },
                                options: { responsive: true, maintainAspectRatio: false }
                            });
                        } else {
                            ctxP.clearRect(0, 0, canvasP.width, canvasP.height);
                            ctxP.font = '16px sans-serif';
                            ctxP.textAlign = 'center';
                            ctxP.fillText('無資料', canvasP.width / 2, canvasP.height / 2);
                        }
                    })
                    .catch(err => console.error(err));
            });
        });
    </script>

</asp:Content>





