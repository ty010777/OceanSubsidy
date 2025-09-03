let current = Date.now();

export const isProjectEditable = (type, status, step) => {
    const store = useProgressStore();

    if ([1, 14].includes(store[type].status)) { //編輯中,補正補件
        return true;
    }

    if ([1, 4].includes(store[type].changeStatus)) { //變更申請: 編輯中,退回修改
        return true;
    }

    if (store[type].status === 42) { //計畫書修正中
        switch (type) {
            case "culture":
                if ([2, 3, 5].includes(step)) {
                    return true;
                }
                break;
            case "multiple":
                if ([2, 3, 4, 5].includes(step)) {
                    return true;
                }
                break;
            case "accessibility":
                if ([2, 3, 4, 5].includes(step)) {
                    return true;
                }
                break;
        }
    }

    return false;
};

export const nextId = () => `id--${current++}`;

export const toInt = (value) => parseInt(value) || 0;
