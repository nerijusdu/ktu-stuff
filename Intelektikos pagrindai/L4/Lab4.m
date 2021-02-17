clear all;

delimiters = {'.','?','!',',',';',':','/','<','>','-','*','+','-', '[',']','&','_','(',')','=',' ','#','%','@','^','\f','\n','\r', '\t','\v','\\','\0','','{','}','\b','\a'};

pr = 0.5;
N = 8;

files = dir('Spamas');
files = files(3:size(files,1),1);
SpamPath = string(zeros(size(files)));
n = size(files,1);
for i = 1:n
    SpamPath(i,1) = strcat('Spamas\', files(i,1).name);
end

files  =  dir('Ne_spamas');
files = files(3:size(files,1),1);
NoSpamPath = string(zeros(size(files)));
m = size(files,1);
for i = 1:m
    NoSpamPath(i,1) = strcat('Ne_spamas\', files(i,1).name);
end

parts = 10;
partSizeN = round(n/parts);
partSizeM = round(m/parts);


output = zeros((partSizeN*9)+(partSizeM*9),1);
target = output;

index = 1;
for k = 0:(parts-2)
    startN = k * partSizeN + 1;
    endN = (k + 1) * partSizeN;
    learnSpam = SpamPath(startN:endN);

    startM = k * partSizeM + 1;
    endM = (k + 1) * partSizeM;
    learnNotSpam = NoSpamPath(startM:endM);

    startTestN = startN + partSizeN;
    if k ~= parts - 2
        endTestN = endN + partSizeN;
    else
        endTestN = size(SpamPath, 1);
    end
    testSpam = SpamPath(startTestN:endTestN);

    startTestM = startM + partSizeM;
    if k ~= parts - 2
        endTestM = endM + partSizeM;
    else
        endTestM = size(NoSpamPath, 1);
    end
    testNotSpam = NoSpamPath(startTestM:endTestM);

    map = Probabilities(learnSpam, learnNotSpam, delimiters);
    
    for i = 1:size(testSpam,1)
        target(index) = 1;
        p = Frequency(testSpam(i), map, N, delimiters);
        if p > pr
            tmp = " Spamas";
            output(index) = 1; 
        else
            tmp = " Nespamas"; 
            output(index) = 0; 
        end
        tmp = strcat("Failo ", testSpam(i), " tikimybe kad yra spam: ", string(p), tmp);
        disp(tmp);
        index = index + 1;
    end

    for i = 1:size(testNotSpam, 1)
        target(index) = 0;
        p = Frequency(testNotSpam(i), map, N, delimiters);
        if p > pr
            tmp = " Spamas"; 
            output(index) = 1;
        else
            tmp = " Nespamas"; 
            output(index) = 0;
        end
        tmp = strcat("Failo ", testNotSpam(i), " tikimybe kad yra spam: ", string(p), tmp);
        disp(tmp);
        index = index + 1;
    end
end


plotconfusion(target',output', 'Klasifikatoriaus tikslumas');

