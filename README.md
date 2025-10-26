Struttura
	DentalStudioScheduler
	└── Controllers
		└── AppointmentController.cs
	
	DentalStudioScheduler.Context
	├── Context
	│	├──Base //Sarebbe sensato in un progetto di libreria
	│	│	├── ContextModelBase.cs
	│	│	└──	CoreContextBase.cs
	│ 	└── DentalContext.cs
	├── Configurations
	│	├──Base //Sarebbe sensato in un progetto di libreria
	│ 	│	└──ModelBase.cs
	│	└── AppointmentConfiguration.cs
	└── Models\
		│ 	└──Base //Sarebbe sensato in un progetto di libreria
	  	│		└──ModelBase.cs
		├── Appointment.cs
		└── Enums.cs

	DentalStudioScheduler.Data
	├── Extensions
	│	└── AppointmentExtentions.cs
	├── Localizations
	│	└── TranslationStrings.cs
	├── Models\
	│	├── ErrorResponse.cs
	│	└── HttpException.cs
	└── ViewModels\
		├── Filters\
		│   └── FilterAppointmentViewModel.cs
		└── AppointmentViewModel.cs
	
	DentalStudioScheduler.Services\
	│ 		└── AppointmentService.cs

	├── Web
	│   │   
	│   │
	│   ├── 
	│   
	├── Test
	│   │   
	│   │
	│   ├── 
