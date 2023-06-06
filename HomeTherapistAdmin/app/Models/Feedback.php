<?php

namespace App\Models;

use Backpack\CRUD\app\Models\Traits\CrudTrait;
use Illuminate\Database\Eloquent\Factories\HasFactory;
use Illuminate\Database\Eloquent\Model;
use Illuminate\Database\Eloquent\Relations\BelongsTo;

class Feedback extends Model
{
    use CrudTrait;
    use HasFactory;

    protected $table = 'feedbacks';

    public function user(): BelongsTo
    {
        return $this->belongsTo(User::class, 'user_id', 'staff_id');
    }
    public function order(): BelongsTo
    {
        return $this->belongsTo(Order::class);
    }
}
