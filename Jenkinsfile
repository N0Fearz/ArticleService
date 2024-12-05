pipeline {
  agent any
  tools {
    dotnetsdk 'SDK 8'
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
        sh 'dotnet restore ArticleService.sln'
      }
    }
      
    stage('Clean'){
      steps{
        sh 'dotnet clean ArticleService.sln'
      }
    }
      
    stage('Build'){
      steps{
        sh 'dotnet build ArticleService.sln --configuration Release'
      }
    }
    stage('SonarQube Analysis') {
        steps {
          withSonarQubeEnv('SonarQube') { // Naam van de SonarQube server zoals ingesteld in Jenkins
            sh '''
              dotnet sonarscanner begin \
                      /k:"articleservice" \

                    dotnet build --configuration Release

                    dotnet sonarscanner end
                    '''
                }
            }
        }
  }
}
