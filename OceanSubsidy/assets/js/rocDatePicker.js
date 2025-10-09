/**
 * 民國年日期選擇器模組 (超簡化版)
 * 配合 flatpickr-roc-custom.js 使用
 * 完全與 ASP.NET 相容
 */

(function() {
    'use strict';

    // 民國年轉換工具
    const RocDateUtils = {
        /**
         * 西元年轉民國年
         */
        adToRoc: function(adYear) {
            return adYear - 1911;
        },

        /**
         * 民國年轉西元年
         */
        rocToAd: function(rocYear) {
            return rocYear + 1911;
        },

        /**
         * 格式化民國年日期字串
         */
        formatRocDate: function(date) {
            if (!date || !(date instanceof Date)) return '';

            const rocYear = this.adToRoc(date.getFullYear());
            const month = (date.getMonth() + 1).toString().padStart(2, '0');
            const day = date.getDate().toString().padStart(2, '0');

            // 使用三位數民國年格式，與後端一致
            return `${rocYear.toString().padStart(3, '0')}/${month}/${day}`;
        },

        /**
         * 解析民國年日期字串
         */
        parseRocDate: function(rocDateStr) {
            if (!rocDateStr || typeof rocDateStr !== 'string') return null;

            const parts = rocDateStr.split('/');
            if (parts.length !== 3) return null;

            const rocYear = parseInt(parts[0], 10);
            const month = parseInt(parts[1], 10);
            const day = parseInt(parts[2], 10);

            if (isNaN(rocYear) || isNaN(month) || isNaN(day)) return null;

            const adYear = this.rocToAd(rocYear);
            const date = new Date(adYear, month - 1, day);

            // 驗證日期是否有效
            if (date.getFullYear() === adYear &&
                date.getMonth() === month - 1 &&
                date.getDate() === day) {
                return date;
            }

            return null;
        }
    };

    // ASP.NET 表單事件觸發器
    function triggerAspNetChange(element) {
        if (!element) return;

        // 標記正在觸發事件，避免無限循環
        if (element._triggering) return;
        element._triggering = true;

        try {
            const changeEvent = new Event('change', { bubbles: true });
            element.dispatchEvent(changeEvent);
        } finally {
            setTimeout(() => {
                element._triggering = false;
            }, 10);
        }
    }

    // 實例追蹤
    const instances = new WeakMap();

    // 清理所有 Flatpickr 實例
    window.cleanupRocDatePickers = function() {
        const elements = document.querySelectorAll('.rocDate');
        elements.forEach(function(element) {
            const instance = instances.get(element);
            if (instance && instance.destroy) {
                try {
                    instance.destroy();
                    instances.delete(element);
                } catch (e) {
                    console.warn('清理 Flatpickr 實例時發生錯誤:', e);
                }
            }
            element.removeAttribute('data-roc-datepicker-initialized');
            element._triggering = false;
        });
    };

    // 民國年日期選擇器初始化
    window.initRocDatePicker = function(selector) {
        const elements = document.querySelectorAll(selector || '.rocDate');

        elements.forEach(function(element) {
            // 檢查是否已經初始化
            if (element.getAttribute('data-roc-datepicker-initialized') === 'true' &&
                instances.has(element)) {
                return;
            }

            // 清理舊實例
            const oldInstance = instances.get(element);
            if (oldInstance && oldInstance.destroy) {
                try {
                    oldInstance.destroy();
                } catch (e) {
                    console.warn('清理舊實例錯誤:', e);
                }
            }

            // 取得初始值
            let initialDate = null;
            if (element.value) {
                initialDate = RocDateUtils.parseRocDate(element.value);
            }

            // Flatpickr 設定 - 年份顯示已在 flatpickr.js 中直接修改
            const fp = flatpickr(element, {
                locale: 'zh_tw',
                allowInput: true,
                clickOpens: true,
                defaultDate: initialDate,

                // 強制使用民國年格式化
                formatDate: function(date, format, locale) {
                    // 始終返回民國年格式，不管請求的格式是什麼
                    return RocDateUtils.formatRocDate(date);
                },

                // 自訂解析函數
                parseDate: function(datestr, format) {
                    const rocDate = RocDateUtils.parseRocDate(datestr);
                    if (rocDate) {
                        return rocDate;
                    }

                    const standardDate = new Date(datestr);
                    if (!isNaN(standardDate.getTime())) {
                        return standardDate;
                    }

                    return null;
                },

                // 日期改變時
                onChange: function(selectedDates, dateStr, instance) {
                    if (selectedDates.length > 0 && !instance.input._triggering) {
                        const rocDateStr = RocDateUtils.formatRocDate(selectedDates[0]);
                        instance.input.value = rocDateStr;
                        triggerAspNetChange(instance.input);
                    }
                },

                // 關閉時確保格式正確
                onClose: function(selectedDates, dateStr, instance) {
                    if (selectedDates.length > 0 && !instance.input._triggering) {
                        const rocDateStr = RocDateUtils.formatRocDate(selectedDates[0]);
                        instance.input.value = rocDateStr;
                        triggerAspNetChange(instance.input);
                    }
                },

                // 開啟時解析現有值
                onOpen: function(selectedDates, dateStr, instance) {
                    if (instance.input.value && selectedDates.length === 0) {
                        const parsedDate = RocDateUtils.parseRocDate(instance.input.value);
                        if (parsedDate) {
                            instance.setDate(parsedDate, false);
                        }
                    }
                },

                // 值更新時確保格式正確
                onValueUpdate: function(selectedDates, dateStr, instance) {
                    if (selectedDates.length > 0 && !instance.input._triggering) {
                        const rocDateStr = RocDateUtils.formatRocDate(selectedDates[0]);
                        if (instance.input.value !== rocDateStr) {
                            instance.input.value = rocDateStr;
                        }
                    }
                }
            });

            // 儲存實例引用
            instances.set(element, fp);

            // 如果有初始值，設定為民國年格式
            if (initialDate) {
                element.value = RocDateUtils.formatRocDate(initialDate);
            }

            // 強化的焦點處理
            element.addEventListener('focus', function(event) {
                // 確保焦點時直接開啟日曆，而不是停留在 input
                if (fp && !fp.isOpen) {
                    setTimeout(() => {
                        fp.open();
                    }, 10);
                }
            });

            // 基本的手動輸入處理
            element.addEventListener('blur', function(event) {
                if (this._triggering || event.isTrusted === false) return;

                const currentValue = this.value;
                if (currentValue && fp) {
                    const parsedDate = RocDateUtils.parseRocDate(currentValue);
                    if (parsedDate) {
                        fp.setDate(parsedDate, false);
                        this.value = RocDateUtils.formatRocDate(parsedDate);
                        triggerAspNetChange(this);
                    }
                }
            });

            // 輸入事件處理
            element.addEventListener('input', function(event) {
                if (this._triggering) return;

                // 如果輸入的不是預期格式，暫時允許但不觸發變更
                const currentValue = this.value;
                const parsedDate = RocDateUtils.parseRocDate(currentValue);

                if (parsedDate && fp) {
                    fp.setDate(parsedDate, false);
                    // 確保值保持民國年格式
                    this.value = RocDateUtils.formatRocDate(parsedDate);
                    triggerAspNetChange(this);
                }
            });

            // Enter 鍵處理
            element.addEventListener('keydown', function(e) {
                if (e.key === 'Enter') {
                    e.preventDefault();
                    this.blur();
                    if (fp) {
                        fp.close();
                    }
                }
            });

            element.setAttribute('data-roc-datepicker-initialized', 'true');
        });
    };

    // 全域方法
    window.getRocDateValue = function(element) {
        return element ? element.value || '' : '';
    };

    window.setRocDateValue = function(element, rocDateStr) {
        if (!element) return;

        const parsedDate = RocDateUtils.parseRocDate(rocDateStr);
        const instance = instances.get(element);

        if (parsedDate && instance) {
            instance.setDate(parsedDate, false);
            element.value = rocDateStr;
        } else {
            element.value = rocDateStr || '';
        }
    };

    // 工具方法匯出到全域
    window.RocDateUtils = RocDateUtils;

    console.log('[ROC DatePicker] 已載入 - 年份顯示已直接整合至 flatpickr.js 中');

})();