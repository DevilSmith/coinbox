## Requirements

* .NET 8 SDK

## Starting
For installation of application:
1. Download application:
```bash
git clone https://github.com/DevilSmith/coinbox.git
cd coinbox
```
## Usage

#### Coinbox-service

1. Run coinbox-service from ./coinbox:
```bash
cd coinbox-service
dotnet run
```
#### Coinbox-client
1. Go coinbox-client project folder from ./coinbox:
```bash
cd coinbox-client
```
2. Starting in message listening mode:
```bash
dotnet run
```
3. Take coins from coinbox:
```bash
dotnet run take-coins [NUMBER] 
```
4. Get the history of the change in the number of coins:
```bash
dotnet run get-changes [NUMBER] 
```
5. Get the current number of coins:
```bash
dotnet run get-current-number 
```


