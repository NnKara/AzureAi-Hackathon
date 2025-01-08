# Challenge 05 - Observability in Semantic Kernel

 [< Previous Challenge](./Challenge-04.md) - **[Home](../README.md)** - [Next Challenge >](./Challenge-06.md)
 
## Introduction

After implementing your first Semantic Kernel intelligent app, it's time to add observability into your application.

When you build AI solutions, you want to be able to observe the behavior of your services. Observability is the ability to monitor and analyze the internal state of components within a distributed system. It is a key requirement for building enterprise-ready AI solutions.

## Description

Observability is typically achieved through logging, metrics, and tracing. They are often referred to as the three pillars of observability. You will also hear the term "telemetry" used to describe the data collected by these three pillars. Unlike debugging, observability provides an ongoing overview of the system's health and performance.

Semantic Kernel is designed to be observable. It emits logs, metrics, and traces that are compatible to the OpenTelemetry standard. 
You can use the following observability tools to monitor and analyze the behavior of your services built on Semantic Kernel.
- [Console](https://learn.microsoft.com/en-us/semantic-kernel/concepts/enterprise-readiness/observability/telemetry-with-console?tabs=Powershell-CreateFile%2CEnvironmentFile&pivots=programming-language-csharp)
- [Application Insights](https://learn.microsoft.com/en-us/semantic-kernel/concepts/enterprise-readiness/observability/telemetry-with-app-insights?tabs=Powershell&pivots=programming-language-csharp)
- [Aspire Dashboard](https://learn.microsoft.com/en-us/semantic-kernel/concepts/enterprise-readiness/observability/telemetry-with-aspire-dashboard?tabs=Powershell&pivots=programming-language-csharp)

Specifically, Semantic Kernel provides the following observability features:
- Logging: Semantic Kernel logs meaningful events and errors from the kernel, kernel plugins and functions, as well as the AI connectors.
- Metrics: Semantic Kernel emits metrics from kernel functions and AI connectors. You will be able to monitor metrics such as the kernel function execution time, the token consumption of AI connectors, etc.
- Tracing: Semantic Kernel supports distributed tracing. You can track activities across different services and within Semantic Kernel.

You should incorporate observability into your application using Application Insights or Aspire Dashboard. Use the application created in the previous challenge.

## Success Criteria
- Ensure that your application is running and you apply observability with Application inisghts or with Apsire Dashboard
- Inspect the telemetry data by navigating to logs and traces and observe the sequence of calls.
- Demonstrate that you can see through the telemetry the content and history sent to the LLM calls.

## Learning Resources
- [Observability in Semantic Kernel | Microsoft Learn](https://learn.microsoft.com/en-us/semantic-kernel/concepts/enterprise-readiness/observability/?pivots=programming-language-csharp)
- [Inspection of telemetry data with Application Insights | Microsoft Learn](https://learn.microsoft.com/en-us/semantic-kernel/concepts/enterprise-readiness/observability/telemetry-with-app-insights?tabs=Powershell&pivots=programming-language-csharp)
- [Inspection of telemetry data with Aspire Dashboard | Microsoft Learn](https://learn.microsoft.com/en-us/semantic-kernel/concepts/enterprise-readiness/observability/telemetry-with-aspire-dashboard?tabs=Powershell&pivots=programming-language-csharp)