import AccessibilityApplication from "./components/Accessibility/Application.vue";
import AccessibilityAttachment from "./components/Accessibility/Attachment.vue";
import AccessibilityBenefit from "./components/Accessibility/Benefit.vue";
import AccessibilityFunding from "./components/Accessibility/Funding.vue";
import AccessibilityPayment1 from "./components/Accessibility/Payment1.vue";
import AccessibilityPayment2 from "./components/Accessibility/Payment2.vue";
import AccessibilityProgressBar from "./components/Accessibility/ProgressBar.vue";
import AccessibilityWorkSchedule from "./components/Accessibility/WorkSchedule.vue";
import ConfirmModal from "./components/ConfirmModal.vue";
import CommonProgressBar from "./components/CommonProgressBar.vue";
import CultureApplication from "./components/Culture/Application.vue";
import CultureAttachment from "./components/Culture/Attachment.vue";
import CultureFunding from "./components/Culture/Funding.vue";
import CultureOther from "./components/Culture/Other.vue";
import CulturePayment1 from "./components/Culture/Payment1.vue";
import CulturePayment2 from "./components/Culture/Payment2.vue";
import CultureProgress from "./components/Culture/Progress.vue";
import CultureProgressBar from "./components/Culture/ProgressBar.vue";
import CultureWorkSchedule from "./components/Culture/WorkSchedule.vue";
import EducationApplication from "./components/Education/Application.vue";
import EducationAttachment from "./components/Education/Attachment.vue";
import EducationPayment from "./components/Education/Payment.vue";
import EducationProgressBar from "./components/Education/ProgressBar.vue";
import ErrorModal from "./components/ErrorModal.vue";
import HomeDashboard from "./components/HomeDashboard.vue";
import InputBoolean from "./components/InputBoolean.vue";
import InputFile from "./components/InputFile.vue";
import InputInteger from "./components/InputInteger.vue";
import InputMonth from "./components/InputMonth.vue";
import InputRadioGroup from "./components/InputRadioGroup.vue";
import InputSelect from "./components/InputSelect.vue";
import InputText from "./components/InputText.vue";
import InputTextarea from "./components/InputTextarea.vue";
import InputTwDate from "./components/InputTwDate.vue";
import ListPager from "./components/ListPager.vue";
import LiteracyApplication from "./components/Literacy/Application.vue";
import LiteracyAttachment from "./components/Literacy/Attachment.vue";
import LiteracyBenefit from "./components/Literacy/Benefit.vue";
import LiteracyFunding from "./components/Literacy/Funding.vue";
import LiteracyPayment1 from "./components/Literacy/Payment1.vue";
import LiteracyPayment2 from "./components/Literacy/Payment2.vue";
import LiteracyProgressBar from "./components/Literacy/ProgressBar.vue";
import LiteracyWorkSchedule from "./components/Literacy/WorkSchedule.vue";
import MultipleApplication from "./components/Multiple/Application.vue";
import MultipleAttachment from "./components/Multiple/Attachment.vue";
import MultipleBenefit from "./components/Multiple/Benefit.vue";
import MultipleFunding from "./components/Multiple/Funding.vue";
import MultiplePayment1 from "./components/Multiple/Payment1.vue";
import MultiplePayment2 from "./components/Multiple/Payment2.vue";
import MultiplePayment3 from "./components/Multiple/Payment3.vue";
import MultipleProgressBar from "./components/Multiple/ProgressBar.vue";
import MultipleWorkSchedule from "./components/Multiple/WorkSchedule.vue";
import NewsDetail from "./components/NewsDetail.vue";
import NewsList from "./components/NewsList.vue";
import NewsMarquee from "./components/NewsMarquee.vue";
import ProjectChangeReview from "./components/ProjectChangeReview.vue";
import ProjectCorrectionReview from "./components/ProjectCorrectionReview.vue";
import ProjectOrganizer from "./components/ProjectOrganizer.vue";
import ProjectPaymentReview from "./components/ProjectPaymentReview.vue";
import ProjectProgress from "./components/ProjectProgress.vue";
import ProjectReport from "./components/ProjectReport.vue";
import ProjectReportReview from "./components/ProjectReportReview.vue";
import ProjectReview from "./components/ProjectReview.vue";
import ProjectTitle from "./components/ProjectTitle.vue";
import ProjectToolbar from "./components/ProjectToolbar.vue";
import ReportApplyList from "./components/Report/ApplyList.vue";
import ReportApprovedList from "./components/Report/ApprovedList.vue";
import ReportInprogressList from "./components/Report/InprogressList.vue";
import ReportMixChart from "./components/Report/MixChart.vue";
import ReportPieChart from "./components/Report/PieChart.vue";
import RequiredLabel from "./components/RequiredLabel.vue";
import SystemGrantForm1 from "./components/System/GrantForm1.vue";
import SystemGrantForm2 from "./components/System/GrantForm2.vue";
import SystemGrantForm3 from "./components/System/GrantForm3.vue";
import SystemGrantForm4 from "./components/System/GrantForm4.vue";
import SystemGrantList from "./components/System/GrantList.vue";
import SystemNewsList from "./components/System/NewsList.vue";
import SystemNewsForm from "./components/System/NewsForm.vue";
import Tooltip from "./components/Tooltip.vue";
import TwDate from "./components/TwDate.vue";
import { api } from "./composables/api";
import { useGrantStore } from "./stores/grant";
import { useProgressStore } from "./stores/progress";

const components = {
    AccessibilityApplication,
    AccessibilityAttachment,
    AccessibilityBenefit,
    AccessibilityFunding,
    AccessibilityPayment1,
    AccessibilityPayment2,
    AccessibilityProgressBar,
    AccessibilityWorkSchedule,
    ConfirmModal,
    CommonProgressBar,
    CultureApplication,
    CultureAttachment,
    CultureFunding,
    CultureOther,
    CulturePayment1,
    CulturePayment2,
    CultureProgress,
    CultureProgressBar,
    CultureWorkSchedule,
    EducationApplication,
    EducationAttachment,
    EducationPayment,
    EducationProgressBar,
    ErrorModal,
    HomeDashboard,
    InputBoolean,
    InputFile,
    InputInteger,
    InputMonth,
    InputRadioGroup,
    InputSelect,
    InputText,
    InputTextarea,
    InputTwDate,
    ListPager,
    LiteracyApplication,
    LiteracyAttachment,
    LiteracyBenefit,
    LiteracyFunding,
    LiteracyPayment1,
    LiteracyPayment2,
    LiteracyProgressBar,
    LiteracyWorkSchedule,
    MultipleApplication,
    MultipleAttachment,
    MultipleBenefit,
    MultipleFunding,
    MultiplePayment1,
    MultiplePayment2,
    MultiplePayment3,
    MultipleProgressBar,
    MultipleWorkSchedule,
    NewsDetail,
    NewsList,
    NewsMarquee,
    ProjectChangeReview,
    ProjectCorrectionReview,
    ProjectOrganizer,
    ProjectPaymentReview,
    ProjectProgress,
    ProjectReport,
    ProjectReportReview,
    ProjectReview,
    ProjectTitle,
    ProjectToolbar,
    ReportApplyList,
    ReportApprovedList,
    ReportInprogressList,
    ReportMixChart,
    ReportPieChart,
    RequiredLabel,
    SystemGrantForm1,
    SystemGrantForm2,
    SystemGrantForm3,
    SystemGrantForm4,
    SystemGrantList,
    SystemNewsList,
    SystemNewsForm,
    Tooltip,
    TwDate
};

export default {
    api,
    install: (app) => {
        for (const name in components) {
            app.component(name, components[name]);
        }
    },
    useGrantStore,
    useProgressStore
};
