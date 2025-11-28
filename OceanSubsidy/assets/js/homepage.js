document.addEventListener('DOMContentLoaded', function() {
    // 初始化 menu
    initializeMenu(); 

    // 初始化回頂端按鈕
    initializeGoTop();

    // 初始化滾動監聽
    initializeScrollSpy()

    // 初始化漢堡點選後focus
    initializeBurgerFocus();
});

// 初始化 menu
function initializeMenu() {
    // 取得 menu 元素（只查詢一次，避免重複查詢）
    const homepageMenu = document.querySelector('.menu');
    
    // 更新 menu 顯示狀態的函數
    function updateMenu() {
        const windowWidth = window.innerWidth;
        if (windowWidth < 992) {
            // 視窗寬度小於 992px，移除 show class（收起來）
            homepageMenu.classList.remove('show');
        } else {
            // 視窗寬度大於等於 992px，添加 show class（顯示）
            homepageMenu.classList.add('show');
        }
    }

    // 初始化時先執行一次
    updateMenu();

    // 監聽視窗寬度變化事件
    window.addEventListener('resize', function() {
        updateMenu();
    });
}

// 回頂端按鈕
function initializeGoTop() {
    const goTop = document.querySelector('.goTop');
    const a11yTop = document.querySelector('#AU');
    goTop.addEventListener('click', function() {
        window.scrollTo({
            top: 0,
            behavior: 'smooth'
        });
        a11yTop.focus();
    });
}

//滾動監聽
function initializeScrollSpy() {
    const topOffset = 100;
    // 監聽當前視窗滾動事件
    window.addEventListener('scroll', function() {
        if(window.scrollY > topOffset) {
            document.body.classList.add('scroll-down');
        } else {
            document.body.classList.remove('scroll-down');
        }
    });
}


// 漢堡點選後focus
function initializeBurgerFocus() {
    const burger = document.querySelector('.btn-burger');
    const menuLink = document.querySelectorAll('.menu li a');
    burger.addEventListener('click', function() {
        menuLink[0].focus();
    });
}