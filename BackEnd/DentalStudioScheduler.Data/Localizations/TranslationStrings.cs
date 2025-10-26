namespace DentalStudioScheduler.Data.Localization
{
    public class TranslationStrings
    {
        protected TranslationStrings()
        {
        }

        /*- - - - - - - - - A C T I O N - - - - - - - -*/
        public static string ACTIVITY_NOT_FOUND { get; } = "activity.not-found";
        public static string ACTIVITIES_NOT_FOUND { get; } = "activities.not-found";
        public static string ACTIVITY_CODE_DUPLICATE { get; } = "activity.code-duplicate";
        public static string ACTIVITY_CANNOT_DELETE_USED_BY_REGISTRATION { get; } = "activity.cannot-delete-used-by-registration";
        public static string ACTIVITY_CANNOT_INSERT_OBSOLETE { get; } = "activity.cannot-insert-obsolete";
        public static string ACTIVITY_CANNOT_DELETE_USED_BY_GROUP { get; set; } = "activity.cannot-delete-used-by-group";
        public static string ACTIVITY_CANNOT_EDIT_USED_BY_GROUP { get; set; } = "activity.cannot-edit-used-by-group";


        /*- - - - - - - - - C O M M I S S I O N - - - - - - - -*/
        public static string COMMISSION_NOT_FOUND { get; } = "commission.not-found";
        public static string COMMISSIONS_NOT_FOUND { get; } = "commissions.not-found";
        public static string COMMISSION_CODE_DUPLICATE { get; } = "commission.code-duplicate";
        public static string COMMISSION_CANNOT_DELETE_USED_BY_REGISTRATION { get; } = "commission.cannot-delete-used-by-registration";



        /*- - - - - - - - - R E G I S T R A T I O N - - - - - - - -*/
        public static string REGISTRATION_NOT_FOUND { get; } = "registration.not-found";
        public static string REGISTRATIONS_NOT_FOUND { get; } = "registrations.not-found";
        public static string REGISTRATION_MAX_HOURS_EXCEEDED { get; set; } = "registration.max-hours-exceeded";

        /* - - - - - - - - C O M M O N - - - - - - - - - */
        public static string COMMON_DATA_NOT_CORRECT { get; } = "common.data-not-correct";
        public static string COMMON_INVALID_DATA { get; } = "common.invalid-data";
        public static string COMMON_INVALID_PAGE { get; } = "common.invalid-page";
        public static string COMMON_INVALID_PAGE_SIZE { get; } = "common.invalid-page-size";
        public static string COMMON_NOT_FOUND { get; } = "common.not-found";
        public static string COMMON_SERVER_NOT_REACHABLE { get; } = "common.server-not-reachable";
        public static string COMMON_CODE { get; } = "common.code";
        public static string COMMON_DESCRIPTION { get; } = "common.description";
        public static string COMMON_MAINTENANCE_NR { get; } = "common.maintenance-nr";
        public static string COMMON_DATE_CREATE { get; } = "common.date-create";
        public static string COMMON_STATUS { get; } = "common.status";
        public static string COMMON_NOTES { get; } = "common.notes";
        public static string COMMON_DATE_CLOSE { get; } = "common.date-close";
        public static string COMMON_COMPONENTS { get; } = "common.components";
        public static string COMMON_HOURS { get; } = "common.hours";
        public static string COMMON_SORT_DATA_NOT_CORRECT { get; } = "common.sort-data-not-correct";
        public static string COMMON_USER_UNAUTHORIZED { get; } = "common.user-unauthorized";
        public static string COMMON_USER_NO_ROLES { get; } = "common.user-no-token";


        /*- - - - - - - - E R R O R - H E L P  - - - - - -*/
        public static string ERROR_HELP_INVALID_HELP { get; } = "error-help.invalid-help";
        public static string ERROR_HELP_NO_EN_TRANSLATION { get; } = "error-help.no-en-translation";
        public static string ERROR_HELP_REMOVE_TRANSLATION { get; } = "error-help.remove-translation";
        public static string ERROR_HELP_USED_BY_ERROR { get; } = "error-help.used-by-error";

        /*- - - - - - - - A C T I V I T Y - G R O U P - - - - - -*/
        public static string ACTIVITY_GROUP_CANNOT_DELTE_USED_BY_PEOPLE { get; } = "activity-group.cannot-delete-used-by-people";
        public static string ACTIVITY_GROUP_CANNOT_DELTE_USED_BY_ACTIVITY { get; } = "activity-group.cannot-delete-used-by-activity";
        public static string ACTIVITY_GROUP_CANNOT_EDIT_USED_BY_PEOPLE { get; } = "activity-group.cannot-edit-used-by-people";
        public static string ACTIVITY_GROUP_CANNOT_EDIT_USED_BY_ACTIVITY { get; } = "activity-group.cannot-edit-used-by-people";
        public static string ACTIVITY_GROUP_NOT_FOUND { get; } = "activity-group.not-found";
        public static string ACTIVITY_GROUP_CODE_DUPLICATE { get; set; } = "activity-group.code-duplicate";

        /*- - - - - - - - P E O P L E - - - - - -*/
        public static string PEOPLE_CANNOT_DELETE { get; } = "people.cannot-delete";
        public static string PEOPLE_NOT_FOUND { get; } = "people.not-found";
        public static string PEOPLE_CODE_DUPLICATE { get; } = "people.code-duplicate";
        public static string PEOPLE_USER_ALREADY_LINKED { get; set; } = "people.user-already-linked";

    }
}
