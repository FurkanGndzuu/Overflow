

import QuestionDetailedHeader from './QuestionDetailedHeader'
import { getQuestionById } from '@/lib/actions/question-action';

import QuestionContent from './QuestionContent';
import AnswerContent from './AnswerContent';
import AnswersHeader from './AnswersHeader';


type Params = Promise<{id: string}>;

export default async function page({params}: {params: Params}) {
    const {id} = await params;
    
    try {
        const { data: question } = await getQuestionById(id);
        
        if (!question) {
            return <div>Question is not found</div>;
        }
        
        return (
            <div><QuestionDetailedHeader question={question} />
            <QuestionContent question={question} />
            {question.answerCount > 0 && <AnswersHeader answerCount={question.answerCount} />}
            {question.answers.map((answer) => (
                <AnswerContent key={answer.id} answer={answer} />
            ))}
            </div>
        );
    } catch (error) {
        console.error('Hata:', error);
        return <div>Sorular yüklenirken hata oluştu</div>;
    }
}

