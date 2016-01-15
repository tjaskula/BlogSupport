library(ggplot2)

df <- read.csv("data2.csv", sep=" ", header = TRUE, stringsAsFactors = FALSE)
df <- transform(df, Population = as.numeric(gsub(",",".", Population)))
df <- transform(df, Profit = as.numeric(gsub(",",".", Profit)))
df <- transform(df, intercept = as.numeric(gsub(",",".", intercept)))
df <- transform(df, regression = as.numeric(gsub(",",".", regression)))

df1 <- df[,1:2]
df2 <- df[,3:4]

cols <- c("Training Data"="red","Linear Regression"="blue")
ggplot() +
  xlab("Population of City in 10,000s") +
  ylab("Profit in $10,000s") +
  geom_point(data = df1, aes_string(x="Population", y = "Profit", colour = "'Training Data'"), shape = 4, size = 2) +
  geom_line(data = df2, aes_string(x = "intercept", y = "regression", colour="'Linear Regression'"), size = 1) +
  scale_colour_manual(name="",values=cols, breaks=c("Training Data", "Linear Regression"), guide = guide_legend(override.aes=aes(fill=NA))) +
  guides(fill = guide_legend(override.aes = list(linetype = 0, shape=''))
         ,colour = guide_legend(override.aes = list(linetype=c(0, 1)
                                                     , shape=c(4, NA))))