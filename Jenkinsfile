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
            defaultValue: 'refs/head/Create-Dockerfile-and-Jenkinsfile-for-deployment-on-ECS',
            name: 'branch',
            type: 'BRANCH',
            remoteURL: 'https://github.com/xe-gr/data-service.git',
            quickFilterEnabled: true
        )
    }
    environment {
        CLUSTER_NAME="automation-ui"
        AWS_REGION="eu-central-1"
        IMAGE_TAG="latest"
        AWS_ACCOUNT_ID=fetchAwsAccountId("${env.environment}")
        DATE=calculateDate()
        DATE_SHORT=calculateShortenedDate()
        ECR_URL="${AWS_ACCOUNT_ID}.dkr.ecr.${AWS_REGION}.amazonaws.com"
        CLUSTER_ARN="arn:aws:ecs:${AWS_REGION}:${AWS_ACCOUNT_ID}:cluster/${CLUSTER_NAME}"
        DATASERVICE_ARN="arn:aws:ecs:${AWS_REGION}:${AWS_ACCOUNT_ID}:service/dataservice"
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
                    buildDockerImage(env.DATE,env.DATE_SHORT,env.environment,env.ECR_URL,"dataservice")
                }
            }

        }

        stage('Deploy_dataservice') {
            steps {
                script{
                    deployContainerToEcs(env.DATE,env.environment,env.AWS_REGION,"dataservice",env.CLUSTER_ARN,env.WEBSERVER_ARN)
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