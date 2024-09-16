@Library('ECS_deployment_methods')_


pipeline {
    agent { label 'aws' }
    options {
        buildDiscarder(logRotator(numToKeepStr: '10'))
        timestamps()
        ansiColor('xterm')
    }
    parameters{
        choice(
            choices: ['staging', 'production'],
            description: 'Choose the AWS environment',
            name: 'environment'
        )
        listGitBranches(
            branchFilter: '.*',
            defaultValue: 'refs/head/origin/upgrade-dotnet8',
            name: 'branch',
            type: 'BRANCH',
            remoteURL: 'https://github.com/xe-gr/data-service.git',
            credentialsId: '',
            quickFilterEnabled: true
        )
    }
    environment {
        CLUSTER_NAME="xe-automation-desk"
        SERVICE_NAME="dataservice"
        AWS_REGION="eu-central-1"
        IMAGE_TAG="latest"
        AWS_ACCOUNT_ID=fetchAwsAccountId("${env.environment}")
        DATE=calculateDate()
        DATE_SHORT=calculateShortenedDate()
        ECR_URL="${AWS_ACCOUNT_ID}.dkr.ecr.${AWS_REGION}.amazonaws.com"
        CLUSTER_ARN="arn:aws:ecs:${AWS_REGION}:${AWS_ACCOUNT_ID}:cluster/${CLUSTER_NAME}"
        SERVICE_ARN="arn:aws:ecs:${AWS_REGION}:${AWS_ACCOUNT_ID}:service/${CLUSTER_NAME}/${SERVICE_NAME}"
    }
    stages {
        stage('Checkout Source Code') {
            steps {
                script {
                    wrap([$class: 'BuildUser']) {
                        currentBuild.displayName = "${env.BUILD_NUMBER}|${BUILD_USER_ID}|${env.environment}|${env.branch.split("/")[2]}"
                    }
                }
                checkout([$class: 'GitSCM', 
                branches: [[name: env.branch ]], 
                extensions: [], 
                userRemoteConfigs: [[ 
                url: 'https://github.com/xe-gr/data-service.git']]])         
            }
        }
        stage('Pre-deployment check') {
            steps {
                preDeploymentCheck(env.environment,env.branch)
            }
        }
        stage('ECR Login') {
            steps {
                ecrLogin(env.environment,env.ECR_URL)
            }
        }
        stage('Build') {
            steps {
                script{
                    buildDockerImage(env.DATE,env.DATE_SHORT,env.environment,env.ECR_URL,env.SERVICE_NAME)
                }
            }

        }

        stage('Deploy_dataservice') {
            steps {
                script{
                    deployContainerToEcs(env.DATE,env.environment,env.AWS_REGION,env.SERVICE_NAME,env.CLUSTER_ARN,env.SERVICE_ARN)
                    }
                }
        }
    }
    post {
        always {
            cleanWs()
        }
    }
 }