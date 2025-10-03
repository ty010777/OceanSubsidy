// 全域變數
const SciApplication = {
    //#region 初始化
    init: function () {
        this.setupCharacterCount();
        this.bindEvents();
        KeywordManager.init();
    },
    //#endregion

    //#region 字數統計功能
    setupCharacterCount: function () {
        const textareas = document.querySelectorAll('textarea[data-max-length]');
        textareas.forEach(textarea => {
            const maxLength = parseInt(textarea.getAttribute('data-max-length'));
            const charCountDiv = textarea.parentNode.querySelector('.char-count');

            if (charCountDiv) {
                const updateCount = () => {
                    const currentLength = textarea.value.length;
                    const percentage = (currentLength / maxLength) * 100;

                    charCountDiv.innerHTML = `已輸入 ${currentLength} 個字數 (限${maxLength}字)`;

                    // 更新樣式
                    charCountDiv.className = 'char-count';
                    if (percentage >= 100) {
                        charCountDiv.classList.add('danger');
                        textarea.classList.add('field-error');
                    } else if (percentage >= 80) {
                        charCountDiv.classList.add('warning');
                        textarea.classList.remove('field-error');
                    } else {
                        textarea.classList.remove('field-error');
                    }
                };

                textarea.addEventListener('input', updateCount);
                updateCount(); // 初始化
            }
        });
    },
    //#endregion

    //#region 事件綁定
    bindEvents: function () {
        // 綁定「完成本頁，下一步」按鈕的點擊事件
        const submitButton = document.querySelector('input[id*="tab1_btnSubmit"]');
        if (submitButton) {
            submitButton.addEventListener('click', (e) => {
                if (!this.validateSubmitForm()) {
                    e.preventDefault();
                    return false;
                }
            });
        }
    },
    
    //#endregion

    //#region 載入狀態
    showLoading: function (element) {
        element.classList.add('loading');
        element.disabled = true;
    },

    hideLoading: function (element) {
        element.classList.remove('loading');
        element.disabled = false;
    }
    //#endregion
};

//#region 關鍵字管理器
const KeywordManager = {
    keywordCounter: 0,
    minKeywords: 3,
    isDataLoaded: false, // 標記是否已載入資料
    
    //#region 初始化
    init: function () {
        // 不再自動初始化預設欄位，等後端資料
        this.bindEvents();
    },

    // 初始化預設的3個關鍵字欄位
    initializeDefaultKeywords: function() {
        const container = document.getElementById('keywordsContainer');
        if (!container) {
            return;
        }

        // 清空容器
        container.innerHTML = '';
        this.keywordCounter = 0;

        // 建立預設的3個關鍵字欄位
        for (let i = 0; i < this.minKeywords; i++) {
            this.addKeywordField('', '');
        }
    },
    //#endregion

    //#region 事件綁定
    bindEvents: function () {
        const addButton = document.getElementById('btnAddKeyword');
        if (addButton) {
            addButton.addEventListener('click', (e) => {
                e.preventDefault();
                this.addKeywordField();
            });
        }

        // 綁定提交按鈕，確保在提交前更新隱藏欄位
        const submitButtons = document.querySelectorAll('input[id*="tab1_btnTempSave"], input[id*="tab1_btnSubmit"]');
        submitButtons.forEach(button => {
            button.addEventListener('click', () => {
                this.updateHiddenField();
            });
        });
    },
    //#endregion

    //#region 欄位管理
    // 新增關鍵字欄位
    addKeywordField: function (chineseValue = '', englishValue = '') {
        const container = document.getElementById('keywordsContainer');
        if (!container) {
            return;
        }

        this.keywordCounter++;
        const keywordId = this.keywordCounter;

        const keywordRow = document.createElement('div');
        keywordRow.className = 'keyword-row row g-2';
        keywordRow.dataset.keywordId = keywordId;

        const isRequired = this.keywordCounter <= this.minKeywords;
        const requiredStar = isRequired ? '<span class="text-pink view-mode">*</span>' : '';

        keywordRow.innerHTML = `
    <div class="col-12 col-md-5">
        <div class="input-group">
            <span class="input-group-text" style="width: 70px;">
                ${requiredStar}中文
            </span>
            <input type="text" 
                   class="form-control keyword-ch" 
                   placeholder="請輸入" 
                   value="${chineseValue}"
                   data-keyword-id="${keywordId}">
        </div>
    </div>
    <div class="col-12 col-md-5">
        <div class="input-group">
            <span class="input-group-text" style="width: 70px;">
                ${requiredStar}英文
            </span>
            <input type="text" 
                   class="form-control keyword-en" 
                   placeholder="請輸入" 
                   value="${englishValue}"
                   data-keyword-id="${keywordId}">
        </div>
    </div>
    ${!isRequired ? `
    <div class="col-12 col-md-2 d-flex align-items-center">
        <button class="btn btn-sm btn-dark-green2 delete-keyword" 
                type="button" 
                data-keyword-id="${keywordId}"
                data-delete-bound="true"
                title="刪除此關鍵字">
            <i class="fas fa-trash"></i>
        </button>
    </div>` : ''}
`;

        container.appendChild(keywordRow);

        // 綁定輸入事件
        const chInput = keywordRow.querySelector('.keyword-ch');
        const enInput = keywordRow.querySelector('.keyword-en');
        
        chInput.addEventListener('input', () => this.updateHiddenField());
        enInput.addEventListener('input', () => this.updateHiddenField());

        // 綁定刪除按鈕事件
        const deleteBtn = keywordRow.querySelector('.delete-keyword');
        if (deleteBtn) {
            deleteBtn.addEventListener('click', () => this.removeKeywordField(keywordId));
        }

        this.updateHiddenField();
    },

    // 移除關鍵字欄位
    removeKeywordField: function (keywordId) {
        const keywordRow = document.querySelector(`[data-keyword-id="${keywordId}"]`);
        if (keywordRow) {
            keywordRow.remove();
            this.updateHiddenField();
        }
    },
    //#endregion

    //#region 資料載入
    // 載入關鍵字資料
    loadFromData: function (keywordsArray) {
        const container = document.getElementById('keywordsContainer');
        if (!container) {
            return;
        }

        // 清空容器
        container.innerHTML = '';
        this.keywordCounter = 0;

        // 後端保證會給 3 筆資料，直接載入
        if (keywordsArray && Array.isArray(keywordsArray)) {
            keywordsArray.forEach((keyword, i) => {
                const chineseValue = keyword.KeyWordTw || '';
                const englishValue = keyword.KeyWordEn || '';
                
                this.addKeywordField(chineseValue, englishValue);
            });
        }

    },

    // 載入現有關鍵字資料 (提供給後端呼叫)
    loadExistingKeywords: function (keywordsArray) {
        // 檢查是否有容器，如果沒有就等等再試
        const container = document.getElementById('keywordsContainer');
        if (!container) {
            setTimeout(() => this.loadExistingKeywords(keywordsArray), 100);
            return;
        }
        
        this.loadFromData(keywordsArray);
    },
    //#endregion

    //#region 資料處理
    // 收集關鍵字資料
    getKeywordsData: function() {
        const keywords = [];
        const keywordRows = document.querySelectorAll('.keyword-row');
        
        keywordRows.forEach((row) => {
            const chInput = row.querySelector('.keyword-ch');
            const enInput = row.querySelector('.keyword-en');
            
            if (chInput && enInput) {
                const chValue = chInput.value ? chInput.value.trim() : '';
                const enValue = enInput.value ? enInput.value.trim() : '';
                
                keywords.push({
                    KeyWordTw: chValue,
                    KeyWordEn: enValue
                });
            }
        });
        
        return keywords;
    },

    // 更新隱藏欄位
    updateHiddenField: function () {
        const keywordsData = this.getKeywordsData();
        
        // 直接用 ASP.NET 生成的 ClientID 查找隱藏欄位
        let hiddenField = document.querySelector('input[id*="hiddenKeywordsData"]');
        
        if (hiddenField) {
            const jsonString = JSON.stringify(keywordsData);
            hiddenField.value = jsonString;
        }
        
        return keywordsData;
    },
    //#endregion
};
window.KeywordManager = KeywordManager;
//#endregion


// 頁面載入完成後初始化
document.addEventListener('DOMContentLoaded', function () {
    SciApplication.init();
});
