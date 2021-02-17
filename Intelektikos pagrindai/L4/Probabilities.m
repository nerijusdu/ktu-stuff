function map = Probabilities(SpamFiles, NoSpamFiles, delimiters)
    mapSpam = containers.Map('KeyType', 'char', 'ValueType', 'double');
    mapNotSpam = containers.Map('KeyType', 'char', 'ValueType', 'double');
    map = containers.Map('KeyType', 'char', 'ValueType', 'double');

    for i = 1:size(SpamFiles,1)
        f = fileread(char(SpamFiles(i)));
        C = lower(strsplit(f,delimiters))';
        [uniq, ~, j] = unique(C);
        freq = accumarray(j, 1);    
        bool = isKey(mapSpam, uniq);

        for j = 1:length(uniq)
            if (bool(j) == 1), mapSpam(uniq{j}) = mapSpam(uniq{j}) + freq(j);
            else, mapSpam(uniq{j}) = freq(j); end
        end
    end

    for i = 1:size(NoSpamFiles,1)
        f = fileread(char(NoSpamFiles(i)));
        C = lower(strsplit(f,delimiters))';
        [uniq, ~, j] = unique(C);
        freq = accumarray(j, 1);    
        bool = isKey(mapNotSpam, uniq);

        for j = 1:length(uniq)
            if (bool(j) == 1), mapNotSpam(uniq{j}) = mapNotSpam(uniq{j}) + freq(j);
            else, mapNotSpam(uniq{j}) = freq(j); end
        end
    end

    words = unique(horzcat(keys(mapSpam), keys(mapNotSpam)))';
    bool = isKey(mapSpam,words);
    bool2 = isKey(mapNotSpam,words);
    PSW = ones(length(words),1);
    totalS = sum(double(string(values(mapSpam))));
    totalN = sum(double(string(values(mapNotSpam))));

    for i = 1:length(words)
        if bool(i) ~= 1, PSW(i) = 0.01; end
        if bool2(i) ~= 1, PSW(i) = 0.99; end
        if bool(i) && bool2(i)
            PSW(i) = 1/(1+(mapNotSpam(words{i})*totalS)/(mapSpam(words{i})*totalN));
        end
        map(words{i}) = PSW(i);
    end
    end
    