CODE_ANALYSIS_DIR := $(abspath $(CURDIR)/../analytics)

all: clean test

build:
	dotnet build

test:
	dotnet test 

clean:
	dotnet clean

cover:
	dotnet test /p:CollectCoverage=true \
		/p:Include="[Fare*]*" /p:Exclude="[Fare.Integration*]*"  ./Fare.IntegrationTests \
		/p:CoverletOutputFormat=opencover /p:CoverletOutput=$(CODE_ANALYSIS_DIR)/opencover.xml 

	reportgenerator.exe -reports:$(CODE_ANALYSIS_DIR)/opencover.xml -targetdir:$(CODE_ANALYSIS_DIR)

	@echo Code analytics report: $(join $(CODE_ANALYSIS_DIR)/, index.htm)