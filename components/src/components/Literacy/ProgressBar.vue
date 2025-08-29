<template>
    <common-progress-bar :steps="steps" type="literacy"></common-progress-bar>
</template>

<script setup>
    const steps = [
        { step: 1, title: "申請書／計畫書", url: "Application" },
        { step: 2, title: "期程／工作項目／查核", url: "WorkSchedule" },
        { step: 3, title: "經費", url: "Funding" },
        { step: 4, title: "成果效益", url: "Benefit" },
        { step: 5, title: "上傳附件／提送申請", url: "Attachment" }
    ];
</script>
