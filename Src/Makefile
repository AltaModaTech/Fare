SRC_DIR := $(CURDIR)
TEST_DIR := $(abspath $(SRC_DIR)/Fare.IntegrationTests)
CODE_ANALYSIS_DIR := $(abspath $(SRC_DIR)/../analytics)

build:
	dotnet build

all: clean test

test:
	dotnet test 

clean:
	dotnet clean
	rm -fr ${CODE_ANALYSIS_DIR}

cover: build
	for d in $(TEST_DIR)/bin/Debug/* ; do \
		FX=`basename $${d}` ; \
		echo ; \ # echo $${FX} ; \
 		\
		dotnet test --framework=$${FX} /p:CollectCoverage=true \
			/p:Include="[Fare*]*" /p:Exclude="[Fare.Integration*]*"  ./Fare.IntegrationTests \
			/p:CoverletOutputFormat=opencover /p:CoverletOutput=$(CODE_ANALYSIS_DIR)/opencover.xml ; \
 		\
		reportgenerator -reports:$(CODE_ANALYSIS_DIR)/opencover.$${FX}.xml -targetdir:$(CODE_ANALYSIS_DIR)/$${FX}/ ; \
		\
		echo Code analytics report: $(join $(CODE_ANALYSIS_DIR)/$${FX}/, index.htm) ; \
	done
