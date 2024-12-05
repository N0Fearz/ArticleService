pipeline {
  agent {
      docker {
          image 'mcr.microsoft.com/dotnet/sdk:8.0' // Replace with your required .NET SDK version
      }
  }
  stages{
    stage('Clean and checkout'){
      steps{
        cleanWs()
        checkout scm
      }
    }
      
    stage('Restore'){
      steps{
        sh 'dotnet restore src/ArticleService.sln'
      }
    }
      
    stage('Clean'){
      steps{
        sh 'dotnet clean src/ArticleService.sln'
      }
    }
      
    stage('Build'){
      steps{
        sh 'dotnet build src/ArticleService.sln --configuration Release'
      }
    }
  }
}
