Common Use Stuff
===============
SEQ: by default, Seq will use as much cache memory as it can. To limit this, do below:
------------
```
1. open the config file located at %programdata%\seq\seq.json
2. change the SystemRamTarget to something lower, e.g. 0.2 meaning 20%
```
Node Version Manager:
------------
```
nvm ls
nvm use 12.22.7
nvm uninstall <the version number>
nvm ls-remote
nvm install --<the node version>
nvm proxy http://xxx.nycnet:8080
```

Proxy bypass:
------------
```
npm config set registry http://registry.npmjs.org/
npm config set http-proxy http://xxx.nycnet:8080
npm config set https-proxy http://xxx.nycnet:8080
npm config set proxy http://xxx.nycnet:8080
npm set strict-ssl=false
```

EF helpful tips:
--------------
equivalent to "AsNoTracking()"
```
1. AsEnumerable() in your query, eg. context.User.AsEnumberable()
2. Projection selection, eg. context.User.Select(x=>new {}) or context.User.Select(x=>new yourDto{})

```
see query with below:
```
services.AddDbContextPool<HPDConnectContext>(options =>
   options.UseSqlServer(connectionString, sqlServerOptions =>
   sqlServerOptions.CommandTimeout(appSettings.DB_COMMAND_TIMEOUT))
   .UseLoggerFactory(new LoggerFactory(
        new[] { new ConsoleLoggerProvider((_, __) => true, true) }))
        , appSettings.DB_POOL_SIZE);

var toAddList = _context.ChangeTracker.Entries<UnitRegulatoryMechanism>().Where(x => x.State == EntityState.Added).ToList();
var toDeleteList = _context.ChangeTracker.Entries<UnitRegulatoryMechanism>().Where(x => x.State == EntityState.Modified && x.Property(p=>p.IsActive).CurrentValue==false).ToList();
var toUpdateList = _context.ChangeTracker.Entries<UnitRegulatoryMechanism>().Where(x => x.State == EntityState.Modified && x.Property(p => p.IsActive).CurrentValue == true).ToList();
```
Angular cli: ng is not recongnized
-----------
1. open environment variables-->path
2. ensure the nodejs path is at the end, see below:
```
%USERPROFILE%\AppData\Roaming\npm;%USERPROFILE%\AppData\Roaming\npm\node_modules\@angular\cli\bin;%AppData%\npm;C:\Program Files\nodejs\;
```

Npm commands
-----------
### Guarantee exact same version of every package, it reads package-lock.json
```
npm ci 

```
### list installed packages
```
npm list -g --depth 0
```

### install angular cli
```
npm install -g @angular/cli
```

Angular:
------------
### Load config from a json file or remote server
```
//add below to main.ts
fetch('./app-config.json')
.then(res=>res.json())
.then(config=>{
  if (environment.production) {
    enableProdMode();
  }
  platformBrowserDynamic([{provide: APP_CONFIG, useValue: config}]).bootstrapModule(AppModule).catch(err=>console.log(err));
}).catch(e=>{
  alert('failed to init application...');
});
```

```
//create a new type script file
import { InjectionToken } from "@angular/core"
export class AppConfig {
    api: string
  }  
export let APP_CONFIG = new InjectionToken<AppConfig>('APP_CONFIG')
```

```
//use config like below:
export class AuthService {
  constructor(@Inject(APP_CONFIG) private _config: AppConfig) { 
  }
  
```

### Global Error Handler
```

import { Injectable } from '@angular/core';

@Injectable({
    providedIn: 'root'
})
export class LoggingService {

  logError(message: string, stack: string) {
    // Send errors to be saved here
    // The console.log is only for testing this example.
    console.log('LoggingService: ' + message);
  }
}

import { Injectable} from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';

@Injectable({
  providedIn: 'root'
})
export class NotificationService {
  
  constructor(public snackBar: MatSnackBar) { }
  
  showSuccess(message: string): void {
    this.snackBar.open(message);
  }
  
  showError(message: string): void {
    // The second parameter is the text in the button. 
    // In the third, we send in the css class for the snack bar.
    this.snackBar.open(message, 'X', {panelClass: ['error']});
  }
}

import { Injectable } from '@angular/core';
import { HttpErrorResponse } from '@angular/common/http';

@Injectable({
    providedIn: 'root'
})
export class ErrorService {

    getClientMessage(error: Error): string {
        if (!navigator.onLine) {
            return 'No Internet Connection';
        }
        return error.message ? error.message : error.toString();
    }

    getClientStack(error: Error): string {
        return error.stack;
    }

    getServerMessage(error: HttpErrorResponse): string {
        return error.message;
    }

    getServerStack(error: HttpErrorResponse): string {
        // handle stack trace
        return 'stack';
    }
}

import { Injectable } from '@angular/core';
import { 
  HttpEvent, HttpRequest, HttpHandler, 
  HttpInterceptor, HttpErrorResponse 
} from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { retry, catchError } from 'rxjs/operators';

@Injectable()
export class ServerErrorInterceptor implements HttpInterceptor {

  intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {

    return next.handle(request).pipe(
      retry(1),
      catchError((error: HttpErrorResponse) => {
        if (error.status === 401) {
          // refresh token
        } else {
          return throwError(error);
        }
      })
    );    
  }
}


import { ErrorHandler, Injectable, Injector } from '@angular/core';
import { HttpErrorResponse } from '@angular/common/http';

import { LoggingService } from './services/logging.service';
import { ErrorService } from './services/error.service';
import { NotificationService } from './services/notification.service';

@Injectable()
export class GlobalErrorHandler implements ErrorHandler {

    // Error handling is important and needs to be loaded first.
    // Because of this we should manually inject the services with Injector.
    constructor(private injector: Injector) { }

    handleError(error: Error | HttpErrorResponse) {

        const errorService = this.injector.get(ErrorService);
        const logger = this.injector.get(LoggingService);
        const notifier = this.injector.get(NotificationService);

        let message;
        let stackTrace;

        if (error instanceof HttpErrorResponse) {
            // Server Error
            message = errorService.getServerMessage(error);
            stackTrace = errorService.getServerStack(error);
            notifier.showError(message);
        } else {
            // Client Error
            message = errorService.getClientMessage(error);
            stackTrace = errorService.getClientStack(error);
            notifier.showError(message);
        }

        // Always log errors
        logger.logError(message, stackTrace);

        console.error(error);
    }
}

providers: [
  { provide: ErrorHandler, useClass: GlobalErrorHandler },
  { provide: HTTP_INTERCEPTORS, useClass: ServerErrorInterceptor, multi: true }
]
```
