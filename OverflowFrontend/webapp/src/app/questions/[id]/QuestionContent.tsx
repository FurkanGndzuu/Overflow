'use client';

import {Question} from "@/lib/types";
import VotingButtons from "./VotingButtons";
import QuestionFooter from "./QuestionFooter";


export default  function QuestionContent({question}: { question: Question }) {
  
    return (
        <div className='flex border-b pb-3 px-6'>
            <VotingButtons 
               
            />
            <div className='flex flex-col w-full'>
                <div
                    className='flex-1 mt-4 ml-6 prose max-w-none dark:prose-invert'
                    dangerouslySetInnerHTML={{__html: question.context}}
                />
                <QuestionFooter question={question} />
            </div>
            
        </div>
    );
}