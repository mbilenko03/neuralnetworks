using CNTK;
using System.Collections.Generic;
using System.IO;

namespace NeuralNetwork
{
    public class MLP
    {
        //private static string ImageDataFolder = "../../Tests/EndToEndTests/Image/Data";
        string ModelName;
        string DataPath;
        int[] Layer;

        private string modelFile;

        public MLP(string modelName, string dataPath, int[] layer)
        {
            string ModelName = modelName;
            string DataPath = dataPath;
            int[] Layer = layer;

            modelFile = $"{modelName}.model";
        }


        public void TrainAndEvaluate(DeviceDescriptor device, bool forceRetrain)
        {
            var featureStreamName = "features";
            var labelsStreamName = "labels";
            var classifierName = "classifierOutput";

            Function classifierOutput;
            int[] inputSize = new int[] { Layer[0] };
            int imageSize = Layer[0];
            int numClasses = Layer[Layer.Length - 1];


            IList<StreamConfiguration> streamConfigurations = new StreamConfiguration[]
                { new StreamConfiguration(featureStreamName, imageSize), new StreamConfiguration(labelsStreamName, numClasses) };


            // If a model already exists and not set to force retrain, validate the model and return.
            if (File.Exists(modelFile) && !forceRetrain)
            {
                var minibatchSourceExistModel = MinibatchSource.TextFormatMinibatchSource(
                    Path.Combine(DataPath, "Test_cntk_text.txt"), streamConfigurations);

                TestHelper.ValidateModelWithMinibatchSource(modelFile, minibatchSourceExistModel,
                                    inputSize, numClasses, featureStreamName, labelsStreamName, classifierName, device);
                return;
            }


            // build the network
            var input = CNTKLib.InputVariable(inputSize, DataType.Float, featureStreamName);


            // For MLP, we like to have the middle layer to have certain amount of states.



            int hiddenLayerDim = 200;
            var scaledInput = CNTKLib.ElementTimes(Constant.Scalar<float>(0.00390625f, device), input);
            classifierOutput = CreateMLPClassifier(device, numClasses, hiddenLayerDim, scaledInput, classifierName);

            var labels = CNTKLib.InputVariable(new int[] { numClasses }, DataType.Float, labelsStreamName);
            var trainingLoss = CNTKLib.CrossEntropyWithSoftmax(new Variable(classifierOutput), labels, "lossFunction");
            var prediction = CNTKLib.ClassificationError(new Variable(classifierOutput), labels, "classificationError");


            // prepare training data
            var minibatchSource = MinibatchSource.TextFormatMinibatchSource(
                Path.Combine(DataPath, "Train_cntk_text.txt"), streamConfigurations, MinibatchSource.InfinitelyRepeat);

            var featureStreamInfo = minibatchSource.StreamInfo(featureStreamName);

            var labelStreamInfo = minibatchSource.StreamInfo(labelsStreamName);


            // set per sample learning rate
            CNTK.TrainingParameterScheduleDouble learningRatePerSample = new CNTK.TrainingParameterScheduleDouble(
                0.003125, 1);


            IList<Learner> parameterLearners = new List<Learner>() { Learner.SGDLearner(classifierOutput.Parameters(), learningRatePerSample) };

            var trainer = Trainer.CreateTrainer(classifierOutput, trainingLoss, prediction, parameterLearners);

            const uint minibatchSize = 64;
            int outputFrequencyInMinibatches = 20, i = 0;
            int epochs = 5;

            while (epochs > 0)

            {
                var minibatchData = minibatchSource.GetNextMinibatch(minibatchSize, device);
                var arguments = new Dictionary<Variable, MinibatchData>
                {
                    { input, minibatchData[featureStreamInfo] },
                    { labels, minibatchData[labelStreamInfo] }
                };

                trainer.TrainMinibatch(arguments, device);
                TestHelper.PrintTrainingProgress(trainer, i++, outputFrequencyInMinibatches);

                /*
                * MinibatchSource is created with MinibatchSource.InfinitelyRepeat.
                * Batching will not end. Each time minibatchSource completes an sweep (epoch),
                * the last minibatch data will be marked as end of a sweep. We use this flag
                * to count number of epochs.
                */
                if (TestHelper.MiniBatchDataIsSweepEnd(minibatchData.Values))
                {
                    epochs--;
                }
            }

            // Save the trained model
            classifierOutput.Save(modelFile);

            // Validate the model
            var minibatchSourceNewModel = MinibatchSource.TextFormatMinibatchSource(
                Path.Combine(DataPath, "Test_cntk_text.txt"), streamConfigurations, MinibatchSource.FullDataSweep);

            TestHelper.ValidateModelWithMinibatchSource(modelFile, minibatchSourceNewModel,
                                inputSize, numClasses, featureStreamName, labelsStreamName, classifierName, device);
        }

        private static Function CreateMLPClassifier(DeviceDescriptor device, int numOutputClasses, int hiddenLayerDim,
            Function scaledInput, string classifierName)
        {
            Function dense1 = TestHelper.Dense(scaledInput, hiddenLayerDim, device, Activation.Sigmoid, "");
            Function classifierOutput = TestHelper.Dense(dense1, numOutputClasses, device, Activation.None, classifierName);

            return classifierOutput;
        }
    }
}
