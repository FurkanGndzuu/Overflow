'use client'

import {Answer} from "@/lib/types";
import { fuzzyTimeAgo } from "@/lib/utils";
import {Avatar} from "@heroui/avatar";



type Props = {
    answer: Answer;

}

export default function AnswerFooter({ answer }: Props) {
  
    
    return (
        <div className='flex justify-between mt-4'>
            <div className='flex items-center mt-auto'>
               
            </div>
            <div className='flex flex-col basis-2/5 bg-primary/10 px-3 py-2 gap-2 rounded-lg'>
                <span className='text-sm font-extralight'>answered {fuzzyTimeAgo(answer.createdAt)}</span>
                <div className='flex items-center gap-3'>
                    <Avatar className='h-6 w-6' color='secondary'
                            name={answer.userDisplayName?.charAt(0)} />
                    <div className='flex flex-col items-center'>
                        <span>{answer.userDisplayName}</span>
                        <span className='self-start text-sm font-semibold'>
                            {answer.userDisplayName}
                        </span>
                    </div>
                </div>
            </div>
        </div>
    );
}