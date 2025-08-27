// 切換密碼顯示狀態
function togglePasswordVisibility() {
    const passwordInput = document.getElementById('password');
    const eyeOpenButton = document.querySelector('.eye-open');
    const eyeCloseButton = document.querySelector('.eye-close');


    const isVisible = passwordInput.type === 'password';
    passwordInput.type = isVisible ? 'text' : 'password';
    eyeOpenButton.style.display = isVisible ? 'block' : 'none';
    eyeCloseButton.style.display = isVisible ? 'none' : 'block';
}


//電腦版收合
function toggleMenu() {
    document.querySelector('.mis-layout').classList.toggle("isClose");
}

// 小於1024自動關閉右側選單
window.onload = function() {
    navResizeState();
    window.addEventListener('resize', navResizeState);
};

// 小於1024自動關閉右側選單
function navResizeState(){
    const windowWidth = window.innerWidth;
    if(windowWidth >= 768 && windowWidth <= 1200){
        document.querySelector('.mis-layout').classList.add("isClose");
    }else{
        document.querySelector('.mis-layout').classList.remove("isClose");
    }
}

// 資訊系統次選單：當子選單項目為 active 時，展開父級選單
function informationSystemSubMenu() {
    const activeSubMenuItems = document.querySelectorAll('.sub-menu li.active');
    
    activeSubMenuItems.forEach(activeItem => {
        // 為父級選單項目新增 active 狀態
        const parentMenuItem = activeItem.closest('li:has(.sub-menu)');
        if (parentMenuItem) {
            parentMenuItem.classList.add('active');
        }
        
        // 展開子選單容器
        const subMenuContainer = activeItem.closest('.collapse');
        if (subMenuContainer) {
            subMenuContainer.classList.add('show');
            
            // 設定觸發按鈕的 aria-expanded 屬性
            const triggerButton = document.querySelector(`[aria-controls="${subMenuContainer.id}"]`);
            if (triggerButton) {
                triggerButton.setAttribute('aria-expanded', 'true');
            }
        }
    });
}

// DOM 載入完成後執行
document.addEventListener('DOMContentLoaded', function() {
    // 初始化資訊系統子選單
    informationSystemSubMenu();
    
    // 初始化 Bootstrap 5 Tooltip - 位置朝上
    const tooltipTriggerList = document.querySelectorAll('[data-bs-toggle="tooltip"]');
    const tooltipList = [...tooltipTriggerList].map(tooltipTriggerEl => 
        new bootstrap.Tooltip(tooltipTriggerEl, {
            placement: 'top', // 固定朝上顯示
            trigger: 'hover' // 滑鼠懸停和焦點觸發
        })
    );
    
    
    // 初始化自動調整高度的文本區域
    initializeAutoResizeTextareas();
    
    // 初始化聲明書閱讀滾動事件
    initializeAgreementScroll();
    
    // 初始化錨點選單
    initializeAnchorMenu();
    
    // 初始化滾動監聽
    initializeScrollSpy();
    
    // 初始化 Tom Select
    initializeTomSelect();

});


// ===================================================
// 自動調整高度的文本區域功能
function initializeAutoResizeTextareas() {
    const textareas = document.querySelectorAll('textarea.textarea-auto-resize');
    
    textareas.forEach(textarea => {
        const handleResize = () => {
            textarea.style.height = 'auto';
            textarea.style.height = textarea.scrollHeight + 'px';
        };
        
        // 初始設定
        handleResize();
        
        // 監聽事件
        textarea.addEventListener('input', handleResize);
        textarea.addEventListener('focus', handleResize);
    });
}
// ===================================================


// ===================================================
//初始化聲明書閱讀滾動事件
//當滾動到最下方時，才解除 checkbox 的 disabled 狀態
function initializeAgreementScroll() {
    const agreementContent = document.getElementById('agreementContent');
    const agreeCheckbox = document.getElementById('agreeTerms');
    
    // 檢查元素是否存在
    if (!agreementContent || !agreeCheckbox) {
        return;
    }
    
    // 初始狀態設為 disabled
    agreeCheckbox.disabled = true;
    agreeCheckbox.checked = false;
    
    // 監聽滾動事件
    agreementContent.addEventListener('scroll', function() {
        handleAgreementScroll(agreementContent, agreeCheckbox);
    });
    
    // 初始檢查是否需要滾動（內容高度小於容器高度時直接啟用）
    checkInitialScrollState(agreementContent, agreeCheckbox);
}

/**
 * 處理聲明書滾動事件
 * @param {HTMLElement} scrollContainer 滾動容器
 * @param {HTMLInputElement} checkbox 同意條款的 checkbox
 */
function handleAgreementScroll(scrollContainer, checkbox) {
    // 計算滾動進度
    const scrollTop = scrollContainer.scrollTop;
    const scrollHeight = scrollContainer.scrollHeight;
    const clientHeight = scrollContainer.clientHeight;
    
    // 容忍 5px 的誤差，避免因為小數點計算導致的問題
    const tolerance = 5;
    const isScrolledToBottom = (scrollTop + clientHeight) >= (scrollHeight - tolerance);
    
    // 根據滾動狀態啟用或禁用 checkbox
    if (isScrolledToBottom) {
        checkbox.disabled = false;
        // 添加視覺提示
        checkbox.parentElement.classList.add('scroll-completed');
        scrollContainer.classList.add('scrolled-to-bottom');
    } else {
        checkbox.disabled = true;
        checkbox.checked = false; // 重新禁用時取消勾選
        checkbox.parentElement.classList.remove('scroll-completed');
        scrollContainer.classList.remove('scrolled-to-bottom');
    }
}

/**
 * 檢查初始滾動狀態
 * 如果內容高度小於或等於容器高度，直接啟用 checkbox
 * @param {HTMLElement} scrollContainer 滾動容器
 * @param {HTMLInputElement} checkbox 同意條款的 checkbox
 */
function checkInitialScrollState(scrollContainer, checkbox) {
    // 等待 DOM 完全載入和樣式計算完成
    setTimeout(() => {
        const scrollHeight = scrollContainer.scrollHeight;
        const clientHeight = scrollContainer.clientHeight;
        
        // 如果內容高度小於或等於容器高度，說明不需要滾動
        if (scrollHeight <= clientHeight) {
            checkbox.disabled = false;
            checkbox.parentElement.classList.add('scroll-completed');
            scrollContainer.classList.add('scrolled-to-bottom');
        }
             }, 100);
 }
// ===================================================


// ===================================================
// 錨點選單平滑滑動
function initializeAnchorMenu() {
    document.querySelectorAll('.anchor-wrapper .anchor-menu-item').forEach(item => {
        item.addEventListener('click', handleAnchorClick);
    });
}
//處理錨點點擊事件
function handleAnchorClick(e) {
    e.preventDefault();
    
    const targetId = e.currentTarget.getAttribute('href').substring(1);
    const targetElement = document.getElementById(targetId);
    if (!targetElement) return;
    
    // 獲取滾動容器
    const scrollContainer = document.querySelector('.mis-content') || 
                           document.querySelector('.mis-layout') || 
                           document.documentElement;
    
    // 計算頂部偏移量
    const offset = calculateTopOffset();
    
    // 計算滾動位置
    const containerRect = scrollContainer.getBoundingClientRect();
    const targetRect = targetElement.getBoundingClientRect();
    const relativeTop = targetRect.top - containerRect.top;
    const scrollPosition = scrollContainer.scrollTop + relativeTop - offset;
    
    // 執行平滑滾動
    try {
        scrollContainer.scrollTo({
            top: scrollPosition,
            behavior: 'smooth'
        });
    } catch (error) {
        scrollContainer.scrollTop = scrollPosition;
    }
}
//計算頂部偏移量
function calculateTopOffset() {
    const selectors = ['.application-step', '.anchor-wrapper'];
    const baseSpacing = 20;
    
    return selectors.reduce((total, selector) => {
        const element = document.querySelector(selector);
        return total + (element ? element.offsetHeight : 0);
    }, baseSpacing);
}

// 初始化滾動監聽
function initializeScrollSpy() {
    const scrollContainer = document.querySelector('.mis-content') || 
                           document.querySelector('.mis-layout') || 
                           document.documentElement;
    
    let ticking = false;
    
    const updateActive = () => {
        const anchorItems = document.querySelectorAll('.anchor-menu-item');
        const offset = calculateTopOffset();
        let activeItem = null;
        
        // 找到最接近頂部的可見區塊
        anchorItems.forEach(item => {
            const targetId = item.getAttribute('href').substring(1);
            const targetElement = document.getElementById(targetId);
            
            if (targetElement) {
                const rect = targetElement.getBoundingClientRect();
                const containerRect = scrollContainer.getBoundingClientRect();
                const relativeTop = rect.top - containerRect.top;
                
                if (relativeTop <= offset + 50 && relativeTop > -rect.height) {
                    activeItem = item;
                }
            }
        });
        
        // 更新 active 狀態
        anchorItems.forEach(item => item.classList.remove('active'));
        if (activeItem) activeItem.classList.add('active');
    };
    
    scrollContainer.addEventListener('scroll', () => {
        if (!ticking) {
            requestAnimationFrame(() => {
                updateActive();
                ticking = false;
            });
            ticking = true;
        }
    });
    
    updateActive(); // 初始檢查
}
// ===================================================



// Fancybox
Fancybox.bind("[data-fancybox]", {
    // Your custom options
});

// Tom Select 初始化
function initializeTomSelect() {
    // 年度多選下拉選單
    const yearSelect = document.getElementById('year-select');
    if (yearSelect) {
        new TomSelect('#year-select', {
            plugins: ['remove_button'],
            placeholder: '請選擇年度...',
            allowEmptyOption: false,
            closeAfterSelect: false,
            maxItems: null, // 允許選擇所有項目
            render: {
                item: function(data, escape) {
                    return '<div class="item">' + escape(data.text) + '</div>';
                },
                option: function(data, escape) {
                    return '<div class="option">' + escape(data.text) + '</div>';
                }
            }
        });
    }
    
    // 類別多選下拉選單
    const categorySelect = document.getElementById('category-select');
    if (categorySelect) {
        new TomSelect('#category-select', {
            plugins: ['remove_button'],
            placeholder: '請選擇類別...',
            allowEmptyOption: false,
            closeAfterSelect: false,
            maxItems: null, // 允許選擇所有項目
            render: {
                item: function(data, escape) {
                    return '<div class="item">' + escape(data.text) + '</div>';
                },
                option: function(data, escape) {
                    return '<div class="option">' + escape(data.text) + '</div>';
                }
            }
        });
    }
}

