// 等待 DOM 載入完成後再執行
document.addEventListener('DOMContentLoaded', function() {
    // 如果是 ReviewChecklist 頁面，跳過初始化，讓 ReviewChecklist.js 處理
    if (window.location.pathname.includes('ReviewChecklist.aspx')) {
        console.log('ReviewChecklist 頁面，跳過 planAdmJS 初始化');
        return;
    }
    
    // 取得全選按鈕
    const checkAll = document.querySelector('.checkAll')
    // 取得所有計畫項目的 checkbox
    const checkPlan = document.querySelectorAll('.checkPlan')   
    // 取得批次按鈕面板
    const checkPlanBtnPanel = document.querySelector('.checkPlanBtnPanel')

    // 檢查元素是否存在
    if (!checkAll || !checkPlanBtnPanel || checkPlan.length === 0) {
        console.log('必要的元素不存在，跳過初始化');
        return;
    }

    // 檢查是否有任何項目被勾選，並控制批次按鈕面板的顯示
    function toggleBatchButtons() {
        let hasChecked = false
        checkPlan.forEach(function(checkbox) {
            if (checkbox.checked) {
                hasChecked = true
            }
        })
        
        // 如果有任何項目被勾選就顯示，沒有就隱藏
        if (hasChecked) {
            checkPlanBtnPanel.style.display = 'block'
        } else {
            checkPlanBtnPanel.style.display = 'none'
        }
    }

    // 當全選按鈕被點擊時
    checkAll.addEventListener('change', function() {
        // 遍歷所有計畫項目的 checkbox
        checkPlan.forEach(function(item) {
            // 讓每個項目的勾選狀態 = 全選按鈕的勾選狀態
            item.checked = checkAll.checked
        })
        // 更新批次按鈕的顯示狀態
        toggleBatchButtons()
    })

    // 為每個計畫項目的 checkbox 加上監聽事件
    checkPlan.forEach(function(item) {
        item.addEventListener('change', function() {
            // 檢查是否所有項目都被勾選了
            let allChecked = true
            checkPlan.forEach(function(checkbox) {
                if (!checkbox.checked) {
                    allChecked = false;
                    console.log(checkbox.checked)
                }
            })
            // 根據檢查結果設定全選按鈕的狀態
            checkAll.checked = allChecked;
            // 更新批次按鈕的顯示狀態
            toggleBatchButtons()
        })
    })
});


// 滾動控制功能
function handleScroll() {
    // 取得需要的元素
    const misContent = document.querySelector('.mis-content');
    const scrollPanel = document.querySelector('.scroll-bottom-panel');
    
    // 如果元素不存在，直接結束
    if (!misContent || !scrollPanel) {
        return;
    }
    
    // 取得當前滾動位置
    const currentScrollTop = misContent.scrollTop;
    
    // 初始化上次滾動位置
    if (typeof handleScroll.lastScrollTop === 'undefined') {
        handleScroll.lastScrollTop = currentScrollTop;
        return; // 第一次執行時不進行判斷
    }
    
    // 計算滾動距離差異（避免微小滾動造成的誤判）
    const scrollDifference = currentScrollTop - handleScroll.lastScrollTop;
    const minScrollThreshold = 5; // 最小滾動閾值
    
    // 只有滾動距離超過閾值才進行判斷
    if (Math.abs(scrollDifference) >= minScrollThreshold) {
        // 判斷滾動方向
        const isScrollingDown = scrollDifference > 0;
        const isScrollingUp = scrollDifference < 0;
        
        // 動態計算面板高度，保留50px讓標題可見
        const panelHeight = scrollPanel.offsetHeight;
        const visibleHeight = 50; // 保留的可見高度
        const hideBottomValue = `-${panelHeight - visibleHeight}px`;
        
        // 控制 body 的 class 和面板位置
        if (isScrollingDown) {
            // 向下滾動時加入 scroll-down class 並部分隱藏面板
            document.body.classList.add('scroll-down');
            scrollPanel.style.bottom = hideBottomValue;
        } else if (isScrollingUp) {
            // 向上滾動時移除 scroll-down class 並顯示面板
            document.body.classList.remove('scroll-down');
            scrollPanel.style.bottom = '0px';
        }
        
        // 更新上次滾動位置
        handleScroll.lastScrollTop = currentScrollTop;
    }
}

// 初始化滾動監聽
function initScrollListener() {
    const misContent = document.querySelector('.mis-content');
    
    if (misContent) {
        // 使用 throttle 來優化滾動事件效能
        let ticking = false;
        
        const throttledHandleScroll = () => {
            if (!ticking) {
                requestAnimationFrame(() => {
                    handleScroll();
                    ticking = false;
                });
                ticking = true;
            }
        };
        
        // 監聽滾動事件
        misContent.addEventListener('scroll', throttledHandleScroll, { passive: true });
        
        // 監聽視窗大小變化，重新計算面板位置
        window.addEventListener('resize', () => {
            const scrollPanel = document.querySelector('.scroll-bottom-panel');
            if (scrollPanel && document.body.classList.contains('scroll-down')) {
                const panelHeight = scrollPanel.offsetHeight;
                const visibleHeight = 60; // 保留的可見高度
                scrollPanel.style.bottom = `-${panelHeight - visibleHeight}px`;
            }
        }, { passive: true });
        
        // 初始執行一次
        handleScroll();
    }
}
// 初始化
initScrollListener();


// ========================================
// 表格排序功能模組
// ========================================

/**
 * 表格排序管理器
 */
class TableSorter {
    constructor(tableId = 'sortTable') {
        this.tableId = tableId;
        this.table = document.getElementById(tableId);
        this.tbody = this.table?.querySelector('tbody');
        this.sortable = null;
        
        this.init();
    }
    
    /**
     * 初始化排序功能
     */
    init() {
        if (!this.tbody) {
            // console.warn(`表格 ${this.tableId} 未找到或沒有 tbody`);
            return;
        }
        
        this.initSortable();
        this.initTopButtons();
    }
    
    /**
     * 初始化拖曳排序
     */
    initSortable() {
        this.sortable = new Sortable(this.tbody, {
            animation: 150,
            ghostClass: 'sortable-ghost',
            chosenClass: 'sortable-chosen',
            dragClass: 'sortable-drag',
            handle: '.btnDrag',
            filter: '.no-drag',
            
            onStart: (evt) => {
                // console.log('拖曳開始:', evt.item);
            },
            
            onEnd: (evt) => {
                // console.log('拖曳結束:', evt.oldIndex, '->', evt.newIndex);
                this.updateSortNumbers();
                this.saveNewOrder();
            },
            
            onMove: () => true
        });
    }
    
    /**
     * 初始化置頂按鈕
     */
    initTopButtons() {
        const topButtons = this.table.querySelectorAll('.btnTop');
        
        topButtons.forEach(button => {
            button.addEventListener('click', (e) => {
                e.preventDefault();
                e.stopPropagation();
                this.moveToTop(button.closest('tr'));
            });
        });
    }
    
    /**
     * 將行移動到頂部
     */
    moveToTop(row) {
        if (!row || !this.tbody) return;
        
        this.tbody.insertBefore(row, this.tbody.firstChild);
        this.updateSortNumbers();
        this.saveNewOrder();
        
        const planName = row.getAttribute('data-plan-name');
        // console.log('置頂成功:', planName);
    }
    
    /**
     * 更新排序數字
     */
    updateSortNumbers() {
        const rows = this.tbody.querySelectorAll('tr');
        
        rows.forEach((row, index) => {
            const sortCell = row.querySelector('td:first-child');
            if (sortCell) {
                const newNumber = index + 1;
                sortCell.textContent = newNumber;
                sortCell.setAttribute('data-th', `排序: ${newNumber}`);
            }
        });
    }
    
    /**
     * 獲取當前排序資料
     */
    getCurrentOrder() {
        const rows = this.tbody.querySelectorAll('tr');
        const order = [];
        
        rows.forEach((row, index) => {
            const rowId = row.getAttribute('data-id') || 
                         row.querySelector('td:nth-child(3) a')?.textContent || 
                         `row-${index}`;
            
            order.push({
                id: rowId,
                order: index + 1,
                planName: row.getAttribute('data-plan-name')
            });
        });
        
        return order;
    }
    
    /**
     * 保存新的排序到後端
     */
    saveNewOrder() {
        const newOrder = this.getCurrentOrder();
        // console.log('新的排序順序:', newOrder);
        
        // TODO: 實作 AJAX 請求
        // this.sendOrderToServer(newOrder);
    }
    
    /**
     * 發送排序資料到伺服器
     */
    sendOrderToServer(orderData) {
        // 範例 AJAX 請求
        /*
        fetch('/api/update-order', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify(orderData)
        })
        .then(response => response.json())
        .then(data => {
            console.log('排序已保存:', data);
        })
        .catch(error => {
            console.error('保存排序時發生錯誤:', error);
        });
        */
    }
    
    /**
     * 顯示成功訊息
     */
    showSuccessMessage(message = '操作成功') {
        const alertDiv = document.createElement('div');
        alertDiv.className = 'alert alert-success alert-dismissible fade show position-fixed';
        alertDiv.style.cssText = 'bottom: 20px; right: 20px; z-index: 9999; min-width: 300px;';
        alertDiv.innerHTML = `
            <i class="fas fa-check-circle"></i>
            <strong>成功！</strong> ${message}
            <button type="button" class="btn-close mb-0 p-3" data-bs-dismiss="alert"></button>
        `;
        
        document.body.appendChild(alertDiv);
        
        setTimeout(() => {
            if (alertDiv.parentNode) {
                alertDiv.remove();
            }
        }, 3000);
    }
}

// ========================================
// 全域變數和初始化
// ========================================

let tableSorter = null;

/**
 * 初始化表格排序功能
 */
function initTableSorting(tableId = 'sortTable') {
    tableSorter = new TableSorter(tableId);
    return tableSorter;
}

/**
 * 獲取表格排序器實例
 */
function getTableSorter() {
    return tableSorter;
}

// ========================================
// 事件監聽器
// ========================================

document.addEventListener('DOMContentLoaded', function() {
    initTableSorting();
});

// 如果頁面是動態載入的，也可以手動調用
// initTableSorting('sortTable');


// 水平滾動按鈕群組控制
document.addEventListener('DOMContentLoaded', function () {
    const scrollable = document.querySelector('.horizontal-scrollable');
    if (!scrollable) return;

    const btnPrev = scrollable.querySelector('.btn-prev');
    const btnNext = scrollable.querySelector('.btn-next');
    const scrollTarget = scrollable.querySelector('ul');
    const scrollStep = 120; // 每次滾動的像素

    // 檢查按鈕啟用狀態
    function updateButtonState() {
        if (!scrollTarget) return;
        const maxScroll = scrollTarget.scrollWidth - scrollTarget.clientWidth;
        btnPrev.disabled = scrollTarget.scrollLeft <= 0;
        btnNext.disabled = scrollTarget.scrollLeft >= maxScroll - 1;
    }

    // 左滾動
    btnPrev.addEventListener('click', function () {
        scrollTarget.scrollBy({ left: -scrollStep, behavior: 'smooth' });
    });

    // 右滾動
    btnNext.addEventListener('click', function () {
        scrollTarget.scrollBy({ left: scrollStep, behavior: 'smooth' });
    });

    // 滾動時更新按鈕狀態
    scrollTarget.addEventListener('scroll', updateButtonState);

    // 初始狀態
    updateButtonState();

    // 視窗縮放時也要更新
    window.addEventListener('resize', updateButtonState);
});