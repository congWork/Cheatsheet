Common Use Stuff
===============
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