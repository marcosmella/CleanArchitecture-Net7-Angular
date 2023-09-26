import { Injectable, APP_INITIALIZER } from "@angular/core";
import { enviroment } from "../../enviroments/enviroment";

@Injectable({
	providedIn: "root"
})
export class AppConfigService {
    private endpoint: string = enviroment.apiEndPoint;
    constructor() {	}

	loadAppConfig(): string {
		return this.endpoint;
	}
}

export function ConfigFactory(config: AppConfigService): Function {
	return () => config.loadAppConfig();
}

export function init(): any {
	return {
		provide: APP_INITIALIZER,
		useFactory: ConfigFactory,
		deps: [AppConfigService],
		multi: true
	};
}

const ConfigModule = {
	init: init
};

export { ConfigModule };
