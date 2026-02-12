import { getQuestions } from "@/lib/actions/question-action";

import QuestionCard from "./QuestionsCard";
import QuestionHeader from "./QuestionHeader";

export default async function QuestionsPage({searchParams}: {searchParams?: Promise<{tag?:string}>}) {

    const params = await searchParams;
   const { data: questions, error } = await getQuestions(params?.tag) ;

    if(error) {
        throw new Error(error.message);
    }

    return (
        <>
        <QuestionHeader tag={params?.tag} total={questions?.length || 0} />
        {questions?.map(question => (
            <QuestionCard key={question.id} question={question} />
        ))}
        </>
    );
}