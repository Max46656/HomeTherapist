<?php

namespace App\Models;

use Backpack\CRUD\app\Models\Traits\CrudTrait;
use Illuminate\Database\Eloquent\Factories\HasFactory;
use Illuminate\Database\Eloquent\Model;
use Illuminate\Database\Eloquent\Relations\BelongsTo;

class OrderDetail extends Model
{
    use CrudTrait;
    use HasFactory;

    public function order(): BelongsTo
    {
        return $this->belongsTo(Order::class, 'user_id', 'staff_ID');
    }
}