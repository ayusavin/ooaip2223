FROM bitnami/dotnet-sdk:6.0.404-debian-11-r1 as dev

FROM mcr.microsoft.com/dotnet/sdk:6.0.407-bullseye-slim-amd64 as coverage-collector

ENV APP_DIR=/app
ENV COVERAGE_DIR=$APP_DIR/coveragereport

COPY ./src $APP_DIR

# Run tests and generate tests report
WORKDIR $APP_DIR/SpaceBattleTests/
RUN dotnet test --collect:"XPlat Code Coverage" -r $COVERAGE_DIR .

RUN echo '\
      #! /usr/bin/env sh\n\
      cat $APP_DIR/coverage.tar\n\
     ' > /usr/bin/extract_coverage_report && \
     chmod +x /usr/bin/extract_coverage_report

# Install reportgenerator
ARG REPORT_GENERATOR_VERSION=5.1.10

RUN dotnet add package ReportGenerator --version $REPORT_GENERATOR_VERSION

# Create Report
ENV REPORT_DIR=coveragereport

WORKDIR $APP_DIR

RUN dotnet $HOME/.nuget/packages/reportgenerator/$REPORT_GENERATOR_VERSION/tools/net6.0/ReportGenerator.dll \
     -reports:$(find . -name "coverage.cobertura.xml") \
     -targetdir:$REPORT_DIR \
     "-reporttypes:HtmlInline;Badges" \
     "-assemblyfilters:+space-battle;+SpaceBattleGrpc" \
     "-classfilters:+SpaceBattle.*;+SpaceBattleGrpc.Services.*"

RUN tar cf coverage.tar $REPORT_DIR

CMD extract_coverage_report
